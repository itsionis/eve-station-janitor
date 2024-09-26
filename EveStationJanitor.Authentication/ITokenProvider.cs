using EveStationJanitor.Authentication.Tokens;

namespace EveStationJanitor.Authentication;

public interface ITokenProvider
{
    Task<AuthorizedToken?> GetToken();
}