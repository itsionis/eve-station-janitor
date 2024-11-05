namespace EveStationJanitor.Authentication;

public interface IAuthenticationDataProvider
{
    Task<AuthorizedToken?> GetAuthTokens(int eveCharacterId);

    Task SaveCharacterAuthData(AuthorizedToken token, EveSsoCharacterInfo characterInfo);

    Task DeleteCharacter(int characterId);

    Task<bool> EnsureCharacterAuthenticated(int eveCharacterId);
    
    Task<int?> AuthenticateNewCharacter();
}
