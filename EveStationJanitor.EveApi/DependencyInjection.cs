using EveStationJanitor.EveApi.Esi;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using System.Text.Json;

namespace EveStationJanitor.EveApi;

public static class DependencyInjection
{
    public static IServiceCollection AddEveApi(this IServiceCollection services)
    {
        services.AddHttpClient("eve-esi", client =>
        {
            var productInfo = new ProductInfoHeaderValue("EveStationJanitor", "1.0");
            client.DefaultRequestHeaders.UserAgent.Add(productInfo);

            var commentValue = new ProductInfoHeaderValue("(A CLI market opportunity application by 'Ionis en Gravonere')");
            client.DefaultRequestHeaders.UserAgent.Add(commentValue);
        });

        var jsonOptions = new JsonSerializerOptions
        {
            TypeInfoResolver = JsonSourceGeneratorContext.Default
        };

        services.AddKeyedSingleton(JsonSourceGeneratorContext.ServiceKey,jsonOptions);
        services.AddScoped<IPublicEveApi, PublicEveApi>();
        services.AddScoped<IAuthenticatedEveApiProvider, AuthenticatedEveApiProvider>();
        services.AddScoped<EveEsiClient>();
        services.AddScoped<EveEsiRequestFactory>();
        return services;
    }
}
