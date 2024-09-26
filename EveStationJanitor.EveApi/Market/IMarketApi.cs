
using EveStationJanitor.EveApi.Market.Objects;

namespace EveStationJanitor.EveApi.Market;

public interface IMarketApi
{
    Task<EveEsiResult<List<ApiMarketOrder>>> GetMarketOrders(int regionId, int? itemTypeId = null, ApiMarketOrderType orderType = ApiMarketOrderType.All);
}