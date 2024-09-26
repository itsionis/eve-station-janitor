using EveStationJanitor.Core.DataAccess;
using EveStationJanitor.Core.StaticData;
using EveStationJanitor.EveApi;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;

namespace EveStationJanitor.Core;

public static class DependencyInjection
{
    public static IServiceCollection AddCore(this IServiceCollection services, string databaseFilePath)
    {
        services.AddHttpClient("static-data", client =>
        {
            var productInfo = new ProductInfoHeaderValue("EveStationJanitor", "1.0");
            client.DefaultRequestHeaders.UserAgent.Add(productInfo);

            var commentValue = new ProductInfoHeaderValue("(A CLI market opportunity application by 'Ionis en Gravonere')");
            client.DefaultRequestHeaders.UserAgent.Add(commentValue);

            client.BaseAddress = new Uri("https://www.fuzzwork.co.uk/");
        });

        services.AddEveApi();
        services.AddScoped<IEntityTagProvider, EntityTagProvider>();
        services.AddScoped<FuzzworksStaticDataDownloader>();
        services.AddScoped<IEveCharacterData, EveCharacterData>();
        services.AddScoped<IEveMarketOrdersRepository, EveMarketOrdersRepository>();

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
            o.EnableSensitiveDataLogging(true);
#endif
        });

        return services;
    }
}
