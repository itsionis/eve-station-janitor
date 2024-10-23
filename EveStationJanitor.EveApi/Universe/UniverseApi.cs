using EveStationJanitor.EveApi.Esi;
using EveStationJanitor.EveApi.Universe.Objects;

namespace EveStationJanitor.EveApi.Universe;

internal sealed class UniverseApi(EveEsiClient client, EveEsiRequestFactory requestFactory) : IUniverseApi
{
    public async Task<EveEsiResult<ApiItemType>> GetItemType(int id)
    {
        var request = requestFactory.ItemTypeRequest(id);
        var result = await client.Get(request);
        return EveEsiResult.FromResult(result);
    }

    public async Task<EveEsiResult<ApiItemGroup>> GetItemGroup(int id)
    {
        var request = requestFactory.ItemGroupRequest(id);
        var result = await client.Get(request);
        return EveEsiResult.FromResult(result);
    }

    public async Task<EveEsiResult<ApiItemCategory>> GetItemCategory(int id)
    {
        var request = requestFactory.ItemCategoryRequest(id);
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

internal class ItemGroupEndpointSpec(int itemGroupId) : IEveEsiEndpointSpec
{
    public HttpMethod HttpMethod => HttpMethod.Get;

    public string RelativeUrlPath => $"/universe/groups/{itemGroupId}/";

    public object? QueryKeyValues => null;
}

internal class ItemCategoryEndpointSpec(int itemCategoryId) : IEveEsiEndpointSpec
{
    public HttpMethod HttpMethod => HttpMethod.Get;

    public string RelativeUrlPath => $"/universe/categories/{itemCategoryId}/";

    public object? QueryKeyValues => null;
}
