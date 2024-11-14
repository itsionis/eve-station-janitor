using EveStationJanitor.Authentication.Validation;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using System.Net.Http.Headers;
using System.Text.Json;
using EveStationJanitor.Authentication;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddEveOnlineAuthentication(this IServiceCollection services)
    {
        services.AddOptions<EveSsoConfiguration>()
            .BindConfiguration(EveSsoConfiguration.ConfigurationSectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddHttpClient("authentication", client =>
        {
            var productInfo = new ProductInfoHeaderValue("EveStationJanitor", "1.0");
            client.DefaultRequestHeaders.UserAgent.Add(productInfo);

            var commentValue = new ProductInfoHeaderValue("(A CLI market opportunity application by 'Ionis en Gravonere')");
            client.DefaultRequestHeaders.UserAgent.Add(commentValue);

            client.DefaultRequestHeaders.Host = "login.eveonline.com";
        });

        var jsonOptions = new JsonSerializerOptions()
        {
            TypeInfoResolver = JsonSourceGeneratorContext.Default
        };
        
        jsonOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
        services.AddKeyedSingleton(JsonSourceGeneratorContext.ServiceKey, jsonOptions);

        services.AddSingleton<IClock>(SystemClock.Instance);
        services.AddSingleton<IAuthenticationClient, AuthenticationClient>();
        services.AddSingleton<ITokenValidator, TokenValidator>();
        services.AddScoped<IBearerTokenProviderFactory, BearerTokenProviderFactory>();
        
        return services;
    }
}
