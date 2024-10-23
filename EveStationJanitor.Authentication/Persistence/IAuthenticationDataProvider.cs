namespace EveStationJanitor.Authentication.Persistence;

public interface IAuthenticationDataProvider
{
    Task<AuthorizedToken?> GetAuthTokens(int eveCharacterId);

    Task<EveSsoCharacterInfo?> GetCharacterInfo(int eveCharacterId);

    Task WriteCharacterAuthData(int eveCharacterId, AuthorizedToken token, EveSsoCharacterInfo characterInfo);

    Task RemoveCharacter(int characterId);
}
