namespace EveStationJanitor.Authentication;

public interface IAuthenticationClient
{
    public Task<(AuthorizedToken, EveSsoCharacterInfo)?> Authenticate(int? eveCharacterId = null);
    
    Task<(AuthorizedToken, EveSsoCharacterInfo)?> Refresh(AuthorizedToken token);
}
