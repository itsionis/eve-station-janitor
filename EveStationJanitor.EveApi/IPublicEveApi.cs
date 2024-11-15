using EveStationJanitor.EveApi.Market;
using EveStationJanitor.EveApi.Status;
using EveStationJanitor.EveApi.Universe;

namespace EveStationJanitor.EveApi;

public interface IPublicEveApi
{
    IMarketApi Markets { get; }
    IUniverseApi Universe { get; }
    IStatusApi Status { get; }
}
