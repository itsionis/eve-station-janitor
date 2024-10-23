using EveStationJanitor.EveApi.Market;
using EveStationJanitor.EveApi.Universe;

namespace EveStationJanitor.EveApi;

public interface IPublicEveApi
{
    IMarketApi Markets { get; }
    IUniverseApi Universe { get; }
}
