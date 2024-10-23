namespace EveStationJanitor.EveApi.Esi;

internal interface IPagedApiRequest : IApiRequest
{
    public int Page { get; set; }
}
