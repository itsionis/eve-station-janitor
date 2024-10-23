namespace EveStationJanitor.Authentication;

public interface IAuthenticationClient
{
    public Task<(AuthorizedToken, EveSsoCharacterInfo)?> Authenticate();
    Task<(AuthorizedToken, EveSsoCharacterInfo)?> Refresh(AuthorizedToken token);

    public IReadOnlySet<string> Scopes { get; }
}
