using EveStationJanitor.Authentication.Exceptions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NodaTime.Extensions;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace EveStationJanitor.Authentication.Validation;

public class TokenValidator : ITokenValidator
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _clientId;

    // Update keys once per run.
    private readonly Lazy<IList<SecurityKey>> _securityKeys;

    public TokenValidator(IHttpClientFactory httpClientFactory, IOptions<EveSsoConfiguration> configuration)
    {
        _httpClientFactory = httpClientFactory;
        _clientId = configuration.Value.ClientId;
        _securityKeys = new Lazy<IList<SecurityKey>>(FetchSecurityKeys);
    }

    public async Task<(AuthorizedToken, EveSsoCharacterInfo)?> ValidateToken(EveSsoTokens ssoTokens)
    {
        var securityKeys = _securityKeys.Value;
        var handler = new JwtSecurityTokenHandler();

        // https://docs.esi.evetech.net/docs/sso/validating_eve_jwt.html
        var validationParameters = new TokenValidationParameters
        {
            // The SSO metadata endpoint contains a description of the supported operations for the SSO. That endpoint
            // lists the supported endpoints as well as provides a link to the SSO JWT key set which is currently
            // located at https://login.eveonline.com/oauth/jwks. You will need to load that key set using whatever
            // JWT library available. Currently the SSO uses the RS-256 signature method, but will also support ES-256
            // in the near future.
            ValidateIssuerSigningKey = true,
            IssuerSigningKeys = securityKeys,

            // The issuer will either be the host name or the URI of the SSO instance you are using
            // (e.g. "login.eveonline.com" or "https://login.eveonline.com"). Your application should handle looking
            // for both the host name and the URI in the iss claim and should reject any JWT tokens where iss does
            // not equal the host name or URI of EVE’s SSO.
            ValidateIssuer = true,
            ValidIssuers = ["https://login.eveonline.com", "login.eveonline.com"],

            // The aud claim contains the audience and must included both the client_id and the string
            // value: "EVE Online"
            ValidateAudience = true,
            ValidAudiences = ["EVE Online", _clientId],

            // The exp claim contains the expiry date of the token as a UNIX timestamp. You can use that to know when to
            // refresh the token and to make sure that the token is valid.
            ValidateLifetime = true,

            // CCP's servers seem slightly ahead (~1s)
            ClockSkew = TimeSpan.FromSeconds(2),
        };

        handler.ValidateToken(ssoTokens.AccessToken, validationParameters, out var validatedToken);
        if (validatedToken is not JwtSecurityToken jwtValidatedToken)
        {
            return null;
        }

        return await PopulateAuthorizedToken(ssoTokens, jwtValidatedToken);
    }

    private async Task<(AuthorizedToken, EveSsoCharacterInfo)> PopulateAuthorizedToken(EveSsoTokens tokens, JwtSecurityToken validatedToken)
    {
        var nameClaim = validatedToken.Claims.SingleOrDefault(c => c.Type == "name")?.Value ?? "";
        var ownerClaim = validatedToken.Claims.SingleOrDefault(c => c.Type == "owner")?.Value ?? "";
        var expiresOn = validatedToken.ValidTo.ToInstant();

        var subjectClaim = validatedToken.Subject;
        var subjectParts = subjectClaim.Split(':');
        if (!int.TryParse(subjectParts.LastOrDefault(), out var characterId))
        {
            throw new ValidateTokenException($"Could not parse character ID [{subjectParts.LastOrDefault()}] as a numeric Eve Online character ID");
        }

        var returnedScopes = validatedToken.Claims.Where(c => c.Type == "scp").ToList();

        // Get more details on the character
        var client = _httpClientFactory.CreateClient();
        var affiliationResponse = await client.PostAsJsonAsync<List<int>>("https://esi.evetech.net/latest/characters/affiliation/?datasource=tranquility", [characterId]);
        var affiliations = await affiliationResponse.Content.ReadFromJsonAsync<List<CharacterAffiliations>>();
        if (affiliations is null || affiliations.Count == 0)
        {
            throw new ValidateTokenException("Could not acquire character affiliations during token validation");
        }

        var affiliation = affiliations.First();
        var authorizedtoken = new AuthorizedToken
        {
            AccessToken = tokens.AccessToken,
            ExpiresOn = expiresOn,
            RefreshToken = tokens.RefreshToken,
            Scopes = returnedScopes.Select(scope => scope.Value).ToHashSet()
        };

        var characterInfo = new EveSsoCharacterInfo
        {
            CharacterId = characterId,
            CharacterName = nameClaim,
            CharacterOwnerHash = ownerClaim,
            AllianceId = affiliation.AllianceId,
            CorporationId = affiliation.CorporationId,
            FactionId = affiliation.FactionId,
        };

        return (authorizedtoken, characterInfo);
    }
    
    private IList<SecurityKey> FetchSecurityKeys()
    {
        var client = _httpClientFactory.CreateClient();

        var request = new HttpRequestMessage(HttpMethod.Get, EveSsoConstants.JsonWebKeySetsEndpoint);
        var response = client.Send(request);

        string keySetJson;

        using (var reader = new StreamReader(response.Content.ReadAsStream()))
        {
            keySetJson = reader.ReadToEnd();
        }

        var keySet = new JsonWebKeySet(keySetJson);
        return keySet.GetSigningKeys();
    }
    
    private sealed class CharacterAffiliations
    {
        [JsonPropertyName("alliance_id")]
        public int? AllianceId { get; set; }

        [JsonPropertyName("character_id")]
        public int CharacterId { get; set; }

        [JsonPropertyName("corporation_id")]
        public int CorporationId { get; set; }
        
        [JsonPropertyName("faction_id")]
        public int? FactionId { get; set; }
    }
}
