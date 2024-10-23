using Microsoft.IdentityModel.Tokens;

namespace EveStationJanitor.Authentication.Validation;

public interface ITokenValidator
{
    public Task<(AuthorizedToken, EveSsoCharacterInfo)?> ValidateToken(EveSsoTokens ssoTokens);
}
