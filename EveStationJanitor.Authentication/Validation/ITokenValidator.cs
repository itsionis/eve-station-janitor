using EveStationJanitor.Authentication.Tokens;
using Microsoft.IdentityModel.Tokens;

namespace EveStationJanitor.Authentication.Validation;

public interface ITokenValidator
{
    public Task<AuthorizedToken?> ValidateToken(EveSsoTokens ssoTokens);
}
