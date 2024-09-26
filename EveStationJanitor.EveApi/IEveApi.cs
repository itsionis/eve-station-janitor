using EveStationJanitor.EveApi.Character;
using EveStationJanitor.EveApi.Clone;
using EveStationJanitor.EveApi.Market;

namespace EveStationJanitor.EveApi;

public interface IEveApi
{
    ICloneApi Clone { get; }
    IMarketApi Markets { get; }
    ICharacterApi Character { get; }
}
