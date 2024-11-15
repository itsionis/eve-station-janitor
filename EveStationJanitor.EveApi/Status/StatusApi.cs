using EveStationJanitor.EveApi.Esi;
using EveStationJanitor.EveApi.Status.Objects;
using OneOf;
using OneOf.Types;

namespace EveStationJanitor.EveApi.Status;

internal class StatusApi(EveEsiClient client, EveEsiRequestFactory requestFactory) : IStatusApi
{
    public async Task<EveEsiResult<ApiEveServerStatus>> GetServerStatus()
    {
        var request = requestFactory.ServerStatusRequest();
        var result = await client.Get(request);
        return EveEsiResult.FromResult(result);
    }
}

internal class ServerStatusEndpointSpec: IEveEsiEndpointSpec
{
    public HttpMethod HttpMethod => HttpMethod.Get;
    public string RelativeUrlPath => "/status/";
    public object? QueryKeyValues => null;
}
