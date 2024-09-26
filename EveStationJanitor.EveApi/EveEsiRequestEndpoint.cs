namespace EveStationJanitor.EveApi;

internal interface IEveEsiEndpointSpec
{
    HttpMethod HttpMethod { get; }

    string RelativeUrlPath { get; }

    object? QueryKeyValues { get; }
}
