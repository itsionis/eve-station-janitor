using EveStationJanitor.EveApi.Esi;
using EveStationJanitor.EveApi.Market;
using EveStationJanitor.EveApi.Universe;

namespace EveStationJanitor.EveApi;

internal class PublicEveApi(EveEsiClient client, EveEsiRequestFactory requestFactory) : IPublicEveApi
{
    public IMarketApi Markets => new MarketApi(client, requestFactory);

    public IUniverseApi Universe => new UniverseApi(client, requestFactory);
}
