using EveStationJanitor.Authentication.Persistence;
using EveStationJanitor.Authentication.Tokens;
using NodaTime;
using System.Runtime.Versioning;

namespace EveStationJanitor.Authentication;

internal sealed class TokenProvider : ITokenProvider
{
    private readonly IAuthenticationClient _authenticationClient;
    private readonly ITokenPersistence _persistence;
    private readonly IClock _clock;

    public TokenProvider(IAuthenticationClient authenticationClient, ITokenPersistence persistence, IClock clock)
    {
        _authenticationClient = authenticationClient;
        _persistence = persistence;
        _clock = clock;
    }

    [SupportedOSPlatform(platformName: "windows")]
    public async Task<AuthorizedToken?> GetToken()
    {
        var maybePersistedToken = _persistence.ReadTokens();
        if (maybePersistedToken is null)
        {
            // It's null, we need to re-authenticate
            var maybeToken = await _authenticationClient.Authenticate();
            if (maybeToken is null) return null;

            _persistence.WriteTokens(maybeToken);
            return maybeToken;
        }
        else
        {
            // There may be new scopes required since last time, so check them here.
            var missingScopes = _authenticationClient.Scopes.Except(maybePersistedToken.Scopes);
            if (missingScopes.Any())
            {
                var maybeToken = await _authenticationClient.Authenticate();
                if (maybeToken is null) return null;

                _persistence.WriteTokens(maybeToken);
                return maybeToken;
            }

            return await HandlePersistedToken(maybePersistedToken);
        }
    }

    [SupportedOSPlatform(platformName: "windows")]
    private async Task<AuthorizedToken> HandlePersistedToken(AuthorizedToken token)
    {
        // Check expiry 
        var shouldRefreshToken = token.ExpiresOn < _clock.GetCurrentInstant().Plus(Duration.FromMinutes(5));
        if (shouldRefreshToken)
        {
            // Token is expired or will expire shortly.
            var refreshedToken = await _authenticationClient.Refresh(token);
            _persistence.WriteTokens(refreshedToken);
            return refreshedToken;
        }
        else
        {
            // Doesn't need refreshing, just return it
            return token;
        }
    }
}
