using EveStationJanitor.Authentication.Tokens;

namespace EveStationJanitor.Authentication;

public interface IAuthenticationClient
{
    public Task<AuthorizedToken?> Authenticate();
    Task<AuthorizedToken> Refresh(AuthorizedToken token);

    public IReadOnlySet<string> Scopes { get; }
}