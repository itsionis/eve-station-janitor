using EveStationJanitor.Authentication.Persistence;
using NodaTime;

namespace EveStationJanitor.Authentication;

public class BearerTokenProviderFactory : IBearerTokenProviderFactory
{
    private readonly IAuthenticationClient _authenticationClient;
    private readonly IAuthenticationDataProvider _persistence;
    private readonly IClock _clock;

    public BearerTokenProviderFactory(
        IAuthenticationClient authenticationClient,
        IAuthenticationDataProvider persistence,
        IClock clock)
    {
        _authenticationClient = authenticationClient;
        _persistence = persistence;
        _clock = clock;
    }

    public IBearerTokenProvider Create(int characterId)
    {
        return new BearerTokenProvider(
            characterId,
            _authenticationClient,
            _persistence,
            _clock);
    }
}