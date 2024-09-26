using EveStationJanitor.EveApi.Character;
using EveStationJanitor.EveApi.Clone;
using EveStationJanitor.EveApi.Market;

namespace EveStationJanitor.EveApi;

internal sealed class EveApi(EveEsiClient client, EveEsiRequestFactory requestFactory) : IEveApi
{
    public ICloneApi Clone { get; } = new CloneApi(client, requestFactory);
    public IMarketApi Markets { get; } = new MarketApi(client, requestFactory);
    public ICharacterApi Character { get; } = new CharacterApi(client, requestFactory);
}
