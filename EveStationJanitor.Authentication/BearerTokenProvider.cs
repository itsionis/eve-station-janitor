using NodaTime;
using System.Runtime.Versioning;

namespace EveStationJanitor.Authentication;

internal class BearerTokenProvider(int characterId, IAuthenticationClient authenticationClient, IAuthenticationDataProvider persistence, IClock clock) : IBearerTokenProvider
{
    [SupportedOSPlatform(platformName: "windows")]
    public async Task<string?> GetToken()
    {
        var authorizedToken = await GetAuthorizedToken();
        return authorizedToken?.AccessToken;
    }

    private async Task<AuthorizedToken?> GetAuthorizedToken()
    {
        var maybePersistedToken = await persistence.GetAuthTokens(characterId);
        if (maybePersistedToken is null)
        {
            // Re-authenticate if no token
            var maybeToken = await authenticationClient.Authenticate();
            if (maybeToken is null)
            {
                return null;
            }

            var (tokens, characterData) = maybeToken.Value;
            await persistence.SaveCharacterAuthData(tokens, characterData);
            return tokens;
        }

        // Check if token needs refresh
        var shouldRefreshToken = maybePersistedToken.AccessToken == "" || maybePersistedToken.ExpiresOn < clock.GetCurrentInstant().Plus(Duration.FromMinutes(5));
        if (shouldRefreshToken)
        {
            var refreshedToken = await authenticationClient.Refresh(maybePersistedToken);
            if (refreshedToken is null)
            {
                return null;
            }

            var (tokens, characterData) = refreshedToken.Value;
            await persistence.SaveCharacterAuthData(tokens, characterData);
            return tokens;
        }

        return maybePersistedToken;
    }
}