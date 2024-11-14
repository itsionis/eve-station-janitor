using EveStationJanitor.Authentication.Exceptions;
using EveStationJanitor.Authentication.Validation;
using Flurl;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Net;
using System.Security.Cryptography;
using System.Text.Json;
using System.Web;

namespace EveStationJanitor.Authentication;

internal sealed class AuthenticationClient : IAuthenticationClient
{
    private readonly HashSet<string> _scopes = [
        "esi-clones.read_implants.v1",
        "esi-skills.read_skills.v1",
        "esi-characters.read_standings.v1"
    ];
    
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ITokenValidator _tokenValidator;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly string _clientId;
    private readonly string _callbackUrl;
    private readonly TimeSpan _authenticationTimeout;
    private readonly string _state;

    public AuthenticationClient(
        IHttpClientFactory httpClientFactory,
        IOptions<EveSsoConfiguration> configuration,
        ITokenValidator tokenValidator,
        [FromKeyedServices(JsonSourceGeneratorContext.ServiceKey)] JsonSerializerOptions jsonOptions)
    {
        _httpClientFactory = httpClientFactory;
        _clientId = configuration.Value.ClientId;
        _callbackUrl = configuration.Value.CallbackUrl;
        _authenticationTimeout = configuration.Value.AuthenticationTimeout;
        _tokenValidator = tokenValidator;
        _jsonOptions = jsonOptions;
        _state = Guid.NewGuid().ToString();
    }

    public async Task<(AuthorizedToken, EveSsoCharacterInfo)?> Authenticate(int? eveCharacterId)
    {
        var result = await StartAuthCallbackListener(
            HandleAuthCallback,
            HandleAuthorizationCode);

        // Verify the character request is authenticated.
        if (eveCharacterId != null && result != null)
        {
            var characterInfo = result.Value.Item2;
            if (characterInfo.CharacterId != eveCharacterId)
            {
                return null;
            }
        }
        
        return result;
    }

    private async Task<(AuthorizedToken, EveSsoCharacterInfo)?> StartAuthCallbackListener(
        Func<HttpListenerContext, string?> handleAuthCallback,
        Func<string, string, Task<(AuthorizedToken, EveSsoCharacterInfo)?>> handleAuthorizationCode)
    {
        // URL and code verifier are generated simultaneously because the challenge code is part of the URL query
        // parameters.
        var (loginUrl, codeVerifier) = GetAuthorizationUrl();

        string? authorizationCode;

        using (var cts = new CancellationTokenSource())
        using (var listener = new HttpListener())
        {
            listener.Prefixes.Add(_callbackUrl);
            listener.Start();

            OpenEveLoginInBrowser(loginUrl);

            var waitForAuthCallback = listener.GetContextAsync();
            var waitForTimeout = Task.Delay(_authenticationTimeout, cts.Token);
            var completedTask = await Task.WhenAny(waitForAuthCallback, waitForTimeout);

            if (completedTask == waitForAuthCallback)
            {
                cts.Cancel(); // Cancel the timeout task, it's no longer required

                // Keep the server alive during handling of callback in case the callback needs to write
                // data to the response stream.
                var callbackContext = waitForAuthCallback.Result;
                authorizationCode = handleAuthCallback(callbackContext);
            }
            else
            {
                throw new AuthenticationFlowException($"Authentication timed out after {_authenticationTimeout.TotalMinutes} minutes");
            }

            if (authorizationCode == null)
            {
                throw new AuthenticationFlowException("Did not receive authorization code from authentication callback");
            }
        }

        return await handleAuthorizationCode(authorizationCode, codeVerifier);
    }

    private string HandleAuthCallback(HttpListenerContext context)
    {
        // Authorization code needs extracting from the response's query parameters
        var queryString = context.Request.Url?.Query;
        if (queryString is null)
        {
            throw new AuthenticationFlowException("Authentication callback did not include code or state");
        }

        var query = HttpUtility.ParseQueryString(queryString);
        var authorizationCode = query["code"];
        var returnedState = query["state"];

        if ((authorizationCode is null) || (returnedState is null))
        {
            throw new AuthenticationFlowException("Authentication callback did not include code or state");
        }

        // Validate state parameter for security
        if (returnedState != _state)
        {
            throw new AuthenticationFlowException("Invalid state parameter returned from SSO");
        }

        // Update the user's browser letting them know authentication has completed and they can return to the app.
        RespondToCallback(context);

        return authorizationCode;
    }

    public (string authUrl, string codeVerifier) GetAuthorizationUrl()
    {
        var (codeVerifier, codeChallenge) = GenerateCodeChallenge();
        var clientUrl = EveSsoConstants.AuthorizeEndpoint
            .SetQueryParams(new
            {
                client_id = _clientId,
                redirect_uri = _callbackUrl,
                response_type = "code",
                state = _state,
                scope = string.Join(" ", _scopes),
                code_challege = codeChallenge,
                code_challenge_method = "S256",
            });

        return (clientUrl, codeVerifier);
    }

    // Step 4: Exchange authorization code for access token
    public async Task<EveSsoTokens?> ExchangeCodeForTokensAsync(string authorizationCode, string codeVerifier)
    {
        var requestContent = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            {"grant_type", "authorization_code" },
            {"code", authorizationCode },
            {"client_id", _clientId  },
            {"code_verifier", codeVerifier }
        });

        var client = _httpClientFactory.CreateClient("authentication");

        var response = await client.PostAsync(EveSsoConstants.TokenEndpoint, requestContent);
        if (!response.IsSuccessStatusCode)
        {
            // Request failed. We could drill into more details per status code.
            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new AuthenticationFlowException($"Unable to access Eve SSO token endpoint: {errorMessage}");
        }

        try
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var tokens = JsonSerializer.Deserialize<EveSsoTokens>(responseContent, _jsonOptions);
            if (tokens is null)
            {
                throw new AuthenticationFlowException("Did not receive a response when refreshing access token.");
            }

            return tokens;
        }
        catch (Exception e)
        {
            throw new AuthenticationFlowException("Could not parse SSO token response.", e);
        }
    }

    public async Task<(AuthorizedToken, EveSsoCharacterInfo)?> Refresh(AuthorizedToken token)
    {
        var requestContent = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            {"grant_type", "refresh_token" },
            {"refresh_token", token.RefreshToken },
            {"client_id", _clientId  },
        });

        var client = _httpClientFactory.CreateClient("authentication");
        var response = await client.PostAsync(EveSsoConstants.TokenEndpoint, requestContent);
        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new AuthenticationFlowException($"Unable to access Eve SSO token endpoint: {errorMessage}");
        }

        try
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var tokens = JsonSerializer.Deserialize<EveSsoTokens>(responseContent, _jsonOptions);
            if (tokens is null)
            {
                throw new AuthenticationFlowException("Did not receive a response when refreshing access token.");
            }

            var validatedTokens = await _tokenValidator.ValidateToken(tokens);
            return validatedTokens;
        }
        catch (Exception e)
        {
            throw new AuthenticationFlowException("Could not parse SSO token response.", e);
        }
    }

    private async Task<(AuthorizedToken, EveSsoCharacterInfo)?> HandleAuthorizationCode(string authorizationCode, string codeVerifier)
    {
        var token = await ExchangeCodeForTokensAsync(authorizationCode, codeVerifier);
        if (token is null)
        {
            throw new AuthenticationFlowException("Could not exchange authorization code or validate access token");
        }

        return await _tokenValidator.ValidateToken(token);
    }

    private static void RespondToCallback(HttpListenerContext context)
    {
        const string response = "Authentication complete! You can close this window and return to the application.";
        var responseBuffer = Encoding.UTF8.GetBytes(response);

        context.Response.ContentType = "text/plain";
        context.Response.ContentLength64 = responseBuffer.Length;

        using (var output = context.Response.OutputStream)
        {
            output.Write(responseBuffer, 0, responseBuffer.Length);
        }

        context.Response.Close();
    }

    private static (string codeVerifier, string codeChallenge) GenerateCodeChallenge()
    {
        var randomBytes = new byte[32];
        using (var random = RandomNumberGenerator.Create())
        {
            random.GetBytes(randomBytes);
        }

        var codeVerifier = Convert.ToBase64String(randomBytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_'); // Base64 URL encoding

        // Generate SHA-256 hash of the code_verifier
        var challengeBytes = SHA256.HashData(Encoding.UTF8.GetBytes(codeVerifier));
        var codeChallenge = Convert.ToBase64String(challengeBytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_'); // Base64 URL encoding

        return (codeVerifier, codeChallenge);
    }

    private static void OpenEveLoginInBrowser(string loginUrl)
    {
        Process.Start(new ProcessStartInfo { FileName = loginUrl, UseShellExecute = true });
    }
}
