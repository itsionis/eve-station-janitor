using EveStationJanitor.EveApi.Esi;
using System.Net.Http.Headers;
using EveStationJanitor.EveApi;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddEveOnlineApi(this IServiceCollection services)
    {
        services.AddHttpClient("eve-esi", client =>
        {
            var productInfo = new ProductInfoHeaderValue("EveStationJanitor", "1.0");
            client.DefaultRequestHeaders.UserAgent.Add(productInfo);

            var commentValue = new ProductInfoHeaderValue("(A CLI market opportunity application by 'Ionis en Gravonere')");
            client.DefaultRequestHeaders.UserAgent.Add(commentValue);
        });

        services.AddScoped<IPublicEveApi, PublicEveApi>();
        services.AddScoped<IAuthenticatedEveApiProvider, AuthenticatedEveApiProvider>();
        services.AddScoped<EveEsiClient>();
        services.AddScoped<EveEsiRequestFactory>();
        
        return services;
    }
}
