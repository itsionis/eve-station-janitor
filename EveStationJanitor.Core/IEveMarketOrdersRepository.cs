using EveStationJanitor.Core.DataAccess.Entities;
using OneOf.Types;
using OneOf;

namespace EveStationJanitor.Core;

public interface IEveMarketOrdersRepository
{
    Task<OneOf<Success, Error<string>>> LoadOrders(Station station);
    Task<List<AggregatedMarketOrder>> GetAggregateSellOrders(Station station);
    Task<List<AggregatedMarketOrder>> GetAggregateBuyOrders(Station station);
}
