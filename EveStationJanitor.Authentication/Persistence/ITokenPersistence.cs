using System.Runtime.Versioning;
using EveStationJanitor.Authentication.Tokens;

namespace EveStationJanitor.Authentication.Persistence;

public interface ITokenPersistence
{
    AuthorizedToken? ReadTokens();

    void WriteTokens(AuthorizedToken token);
}