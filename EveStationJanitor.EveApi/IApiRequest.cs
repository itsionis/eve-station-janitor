namespace EveStationJanitor.EveApi;

internal interface IApiRequest
{
    public HttpRequestMessage ToRequest();
}

internal interface IPagedApiRequest : IApiRequest
{
    public int Page { get; set; }
}
