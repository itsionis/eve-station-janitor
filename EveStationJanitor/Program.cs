using System.CommandLine;
using System.CommandLine.Parsing;
using EveStationJanitor;
using EveStationJanitor.Authentication;
using EveStationJanitor.Core;
using EveStationJanitor.Core.DataAccess;
using EveStationJanitor.Core.StaticData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var applicationDataDirectoryPath = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
    "Eve Station Janitor");

var databasePath = Path.Combine(applicationDataDirectoryPath, "esj.db");
var tokenPath = Path.Combine(applicationDataDirectoryPath, ".tokens");

Directory.CreateDirectory(applicationDataDirectoryPath);

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddAuth(tokenPath);
builder.Services.AddCore(databasePath);
builder.Services.AddScoped<IAuthenticationDataProvider, DatabaseAuthDataPersistence>();
builder.Services.AddScoped<Janitor>();
builder.Services.AddScoped<App>();

var host = builder.Build();
return await App.Run(host, args);

internal class App(AppDbContext context, Janitor janitor, FuzzworksStaticDataDownloader staticDataLoader)
{
    private const string CharacterOption = "--character";
    private const string TradeHubOption = "--trade-hub";

    public static async Task<int> Run(IHost host, params string[] args)
    {
        var command = new RootCommand();
        var characterOption = new Option<string?>(CharacterOption);
        var tradeHubSystemOption = new Option<TradeHubStation?>(TradeHubOption, parseArgument: ParseTradeHubSystemArgument);

        command.AddOption(characterOption);
        command.AddOption(tradeHubSystemOption);

        command.SetHandler(async (character, tradeHubSystem) =>
        {
            using (var scope = host.Services.CreateScope())
            {
                var app = scope.ServiceProvider.GetRequiredService<App>();
                await app.Run(character, tradeHubSystem);
            }
        }, characterOption, tradeHubSystemOption);

        return await command.InvokeAsync(args);
    }

    private static TradeHubStation? ParseTradeHubSystemArgument(ArgumentResult result)
    {
        if (!result.Tokens.Any())
        {
            return null;
        }

        var token = result.Tokens.SingleOrDefault()?.Value;
        return TradeHubStation.TryGetStationBySystem(token);
    }
    
    private async Task Run(string? characterChoice = null, TradeHubStation? tradeHubStationChoice = null)
    {
        Console.WriteLine("Initialising database...");
        await context.Database.MigrateAsync();

        Console.WriteLine("Loading static data...");
        await staticDataLoader.Run();

        await janitor.Run(characterChoice, tradeHubStationChoice);
    }
}
