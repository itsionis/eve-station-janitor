using EveStationJanitor.Authentication;
using EveStationJanitor.EveApi.Esi;

namespace EveStationJanitor.EveApi.Clone;

internal sealed class CloneApi(EveEsiClient client, EveEsiRequestFactory requestFactory, IBearerTokenProvider tokenProvider) : ICloneApi
{
    public async Task<EveEsiResult<List<int>>> GetActiveCloneImplants(int characterId)
    {
        var request = requestFactory.CloneImplantsRequest(characterId, tokenProvider);
        var result = await client.Get(request);
        return EveEsiResult.FromResult(result);
    }
}

internal class CloneImplantsEndpointSpec(int characterId) : IEveEsiEndpointSpec
{
    public HttpMethod HttpMethod => HttpMethod.Get;

    public string RelativeUrlPath => $"/characters/{characterId}/implants/";

    public object? QueryKeyValues => null;
}
