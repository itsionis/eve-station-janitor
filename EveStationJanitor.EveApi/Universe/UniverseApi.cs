using EveStationJanitor.EveApi.Universe.Objects;

namespace EveStationJanitor.EveApi.Universe;

internal sealed class UniverseApi(EveEsiClient client, EveEsiRequestFactory requestFactory) : IUniverseApi
{
    public async Task<EveEsiResult<ApiItemType>> GetItemType(int itemId)
    {
        var request = requestFactory.ItemTypeRequest(itemId);
        var result = await client.Get(request);
        return EveEsiResult.FromResult(result);
    }
}

internal class ItemTypeEndpointSpec(int itemId) : IEveEsiEndpointSpec
{
    public HttpMethod HttpMethod => HttpMethod.Get;

    public string RelativeUrlPath => $"/universe/types/{itemId}/";

    public object? QueryKeyValues => null;
}
