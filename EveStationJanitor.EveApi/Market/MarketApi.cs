using EveStationJanitor.EveApi.Esi;
using EveStationJanitor.EveApi.Market.Objects;
using System.Globalization;

namespace EveStationJanitor.EveApi.Market;

internal sealed class MarketApi(EveEsiClient client, EveEsiRequestFactory requestFactory) : IMarketApi
{
    public async Task<EveEsiResult<List<ApiMarketOrder>>> GetMarketOrders(int regionId, int? itemTypeId = null, ApiMarketOrderType orderType = ApiMarketOrderType.All)
    {
        var request = requestFactory.MarketOrdersRequest(regionId, itemTypeId, orderType);
        var result = await client.GetPagedCollection(request);
        return EveEsiResult.FromResult(result);
    }
}

internal class MarketOrdersEndpointSpec(int regionId, int? itemTypeId, ApiMarketOrderType orderType) : IEveEsiEndpointSpec
{
    public HttpMethod HttpMethod => HttpMethod.Get;

    public string RelativeUrlPath => $"/markets/{regionId}/orders/";

    public object? QueryKeyValues => new
    {
        type_id = itemTypeId,
        order_type = orderType.ToString().ToLower(CultureInfo.InvariantCulture)
    };
}
