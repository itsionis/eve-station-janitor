using EveStationJanitor.Authentication;
using EveStationJanitor.EveApi.Character;
using EveStationJanitor.EveApi.Clone;
using EveStationJanitor.EveApi.Esi;

namespace EveStationJanitor.EveApi;

internal class AuthenticatedEveApiProvider(EveEsiClient client, EveEsiRequestFactory requestFactory, IBearerTokenProviderFactory tokenProviderFactory) : IAuthenticatedEveApiProvider
{
    public ICloneApi CreateCloneApi(int characterId)
    {
        var tokenProvider = tokenProviderFactory.Create(characterId);
        return new CloneApi(client, requestFactory, tokenProvider);
    }

    public ICharacterApi CreateCharacterApi(int characterId)
    {
        var tokenProvider = tokenProviderFactory.Create(characterId);
        return new CharacterApi(client, requestFactory, tokenProvider);
    }
}
