namespace EveStationJanitor.EveApi.Esi;

internal interface IApiRequest
{
    public HttpRequestMessage ToRequest();
}
