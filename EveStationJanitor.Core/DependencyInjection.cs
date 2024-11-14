using EveStationJanitor.Core.DataAccess;
using EveStationJanitor.Core.StaticData;
using EveStationJanitor.EveApi;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using EveStationJanitor.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddEveStationJanitorCore(this IServiceCollection services, string databaseFilePath)
    {
        services.AddHttpClient("static-data", client =>
        {
            var productInfo = new ProductInfoHeaderValue("EveStationJanitor", "1.0");
            client.DefaultRequestHeaders.UserAgent.Add(productInfo);

            var commentValue = new ProductInfoHeaderValue("(A CLI market opportunity application by 'Ionis en Gravonere')");
            client.DefaultRequestHeaders.UserAgent.Add(commentValue);

            client.BaseAddress = new Uri("https://www.fuzzwork.co.uk/");
        });

        services.AddEveOnlineApi();
        services.AddScoped<IEntityTagProvider, EntityTagProvider>();
        services.AddScoped<FuzzworksStaticDataDownloader>();
        services.AddScoped<IEveMarketOrdersRepository, EveMarketOrdersRepository>();
        services.AddScoped<IEveCharacterDataProvider, EveCharacterDataProvider>();

        var connectionStringBuilder = new SqliteConnectionStringBuilder
        {
            DataSource = databaseFilePath,
            ForeignKeys = true
        };

        services.AddDbContext<AppDbContext>(o =>
        {
            o.UseSnakeCaseNamingConvention();
            o.UseSqlite(connectionStringBuilder.ConnectionString, s =>
            {
                s.UseNodaTime();
            });

#if DEBUG
            o.EnableSensitiveDataLogging();
#endif
        });

        return services;
    }
}
