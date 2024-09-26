using EveStationJanitor.Authentication.Tokens;
using EveStationJanitor.EveApi.Clone.Objects;

namespace EveStationJanitor.EveApi.Clone;

internal sealed class CloneApi(EveEsiClient client, EveEsiRequestFactory requestFactory) : ICloneApi
{
    public async Task<EveEsiResult<ApiCloneImplants>> GetCharacterImplants(int characterId)
    {
        var request = requestFactory.CloneImplantsRequest(characterId);
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
