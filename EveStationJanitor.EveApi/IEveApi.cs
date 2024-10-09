using EveStationJanitor.EveApi.Character;
using EveStationJanitor.EveApi.Clone;
using EveStationJanitor.EveApi.Market;
using EveStationJanitor.EveApi.Universe;

namespace EveStationJanitor.EveApi;

public interface IEveApi
{
    ICloneApi Clone { get; }

    IMarketApi Markets { get; }

    ICharacterApi Character { get; }

    IUniverseApi Universe { get; }
}
