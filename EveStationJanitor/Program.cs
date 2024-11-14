using System.CommandLine;
using EveStationJanitor;
using EveStationJanitor.Authentication;
using EveStationJanitor.Core.DataAccess;
using EveStationJanitor.Core.StaticData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var applicationDataDirectoryPath = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
    "Eve Station Janitor");

var databasePath = Path.Combine(applicationDataDirectoryPath, "esj.db");
Directory.CreateDirectory(applicationDataDirectoryPath);

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddEveOnlineAuthentication();
builder.Services.AddEveStationJanitorCore(databasePath);

builder.Services.AddScoped<IAuthenticationDataProvider, DatabaseAuthDataPersistence>();
builder.Services.AddScoped<Janitor>();
builder.Services.AddScoped<App>();

var host = builder.Build();

var command = new EveStationJanitorCommand(async (character, tradeHubSystem, minimumProfit) =>
{
    using var scope = host.Services.CreateScope();
    var app = scope.ServiceProvider.GetRequiredService<App>();
    await app.Run(character, tradeHubSystem, minimumProfit);
});

return await command.InvokeAsync(args);

internal class App(AppDbContext context, Janitor janitor, FuzzworksStaticDataDownloader staticDataLoader)
{
    public async Task Run(string? characterChoice = null, TradeHubStation? tradeHubStationChoice = null, int minimumProfit = 0)
    {
        Console.WriteLine("Initialising database...");
        await context.Database.MigrateAsync();

        Console.WriteLine("Loading static data...");
        await staticDataLoader.Run();

        await janitor.Run(characterChoice, tradeHubStationChoice, minimumProfit);
    }
}
