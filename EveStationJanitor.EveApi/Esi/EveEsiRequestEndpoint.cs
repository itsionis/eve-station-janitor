namespace EveStationJanitor.EveApi.Esi;

internal interface IEveEsiEndpointSpec
{
    HttpMethod HttpMethod { get; }

    string RelativeUrlPath { get; }

    object? QueryKeyValues { get; }
}
