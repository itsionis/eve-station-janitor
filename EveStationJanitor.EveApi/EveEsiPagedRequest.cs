using EveStationJanitor.Authentication;

namespace EveStationJanitor.EveApi;

internal class EveEsiPagedRequest<TResponse>(IEveEsiEndpointSpec specification, IEntityTagProvider entityTagProvider, ITokenProvider? tokenProvider = null)
{
    public EveEsiRequest<TResponse> ForPage(int page = 1)
    {
        var request = new EveEsiRequest<TResponse>(specification, entityTagProvider, tokenProvider);
        request.AddQueryParameter("page", page);
        return request;
    }
}
