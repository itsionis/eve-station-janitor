using System.Diagnostics;
using EveStationJanitor.Core.DataAccess;
using EveStationJanitor.Core.StaticData;
using EveStationJanitor.EveApi;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;

namespace EveStationJanitor;

using ServerStatus = (bool IsOnline, int? PlayerCount);

internal class App(AppDbContext context, Janitor janitor, FuzzworksStaticDataDownloader staticDataLoader, IPublicEveApi eveApi)
{
    public async Task Run(string? characterChoice = null, TradeHubStation? tradeHubStationChoice = null, int minimumProfit = 0)
    {
        ServerStatus serverStatus = (false, null);

        await AnsiConsole.Status()
            .StartAsync("Checking Tranquility...", async _ => serverStatus = await CheckEveServerStatus());

        if (serverStatus.IsOnline)
        {
            Debug.Assert(serverStatus.PlayerCount is not null);
            AnsiConsole.MarkupLine($"Tranquility is [bold green]online[/] ({serverStatus.PlayerCount} players)");  
        }
        else
        {
            AnsiConsole.MarkupLine("[bold red]Tranquility is offline[/]");
            return;
        }
        
        await AnsiConsole.Status()
            .StartAsync("Initialising...", async _ =>
            {
                await context.Database.MigrateAsync();
                await staticDataLoader.Run();
            });

        await janitor.Run(characterChoice, tradeHubStationChoice, minimumProfit);
    }

    private async Task<ServerStatus> CheckEveServerStatus()
    {
        var serverStatus = await eveApi.Status.GetServerStatus();

        return serverStatus.Match<ServerStatus>(
            response => (true, response.PlayerCount),
            _ => (false, null),
            _ => (false, null));
    }
}
