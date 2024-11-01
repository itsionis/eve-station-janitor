using EveStationJanitor.Core.DataAccess.Entities;
using OneOf.Types;
using OneOf;

namespace EveStationJanitor.Core;

public interface IEveMarketOrdersRepository
{
    Task<OneOf<Success, Error<string>>> LoadOrders(Station station);
    Task<List<MarketOrder>> GetSellOrders(Station station);
    Task<List<MarketOrder>> GetBuyOrders(Station station);
}
