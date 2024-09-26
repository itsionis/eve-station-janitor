using EveStationJanitor.Core.DataAccess;
using EveStationJanitor.Core;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using EveStationJanitor.Core.DataAccess.Entities;
using System.Globalization;

namespace EveStationJanitor;

internal sealed class Janitor(
    AppDbContext context,
    IEveCharacterData eveCharacterData,
    IEveMarketOrdersRepository marketOrdersRepository)
{
    public async Task Run()
    {
        var station = PromptUserForTradeHub();
        if (station is null)
        {
            return;
        }

        PrintPrettyStationName(station);

        var implantsResult = await eveCharacterData.GetImplants();
        if (!implantsResult.TryPickT0(out var _, out var implantError))
        {
            AnsiConsole.Markup($"[red]!! Error fetching implants[/] {implantError.Value}");
            return;
        }

        var skillsResult = await eveCharacterData.GetSkills();
        if (!skillsResult.TryPickT0(out var skills, out var skillsError))
        {
            AnsiConsole.Markup($"[red]!! Error fetching skills[/] {skillsError.Value}");
            return;
        }

        var standingsResult = await eveCharacterData.GetStandings(skills);
        if (!standingsResult.TryPickT0(out var standings, out var standingsError))
        {
            AnsiConsole.Markup($"[red]!! Error fetching standings[/] {standingsError.Value}");
            return;
        }

        // Load regional market orders
        await marketOrdersRepository.LoadOrders(station);

        // Find the most profitable item
        var oreReprocessing = new OreReprocessing(context, skills);
        var stationReprocessing = new StationReprocessing(oreReprocessing, station, skills, standings);

        var profitCalculator = new ProfitCalculator(context, stationReprocessing, skills);
        var flips = await profitCalculator.FindMostProfitableOrders();

        WriteResultsTable(flips);
    }

    private static void WriteResultsTable(IReadOnlyList<ItemFlipAppraisal> flips)
    {
        var table = new Table();

        table.AddColumn("Item");

        table.AddColumn("Group");

        table.AddColumn("Quantity");
        table.Columns[2].RightAligned();

        table.AddColumn("Cost per");
        table.Columns[3].RightAligned();

        table.AddColumn("Total profit");
        table.Columns[4].RightAligned();

        table.AddColumn("Margin");
        table.Columns[5].RightAligned();

        foreach (var flip in flips)
        {
            table.AddRow(
                flip.Item.Name,
                flip.Item.Group.Name,
                flip.Volume.ToString("N0", CultureInfo.CurrentCulture),
                flip.CostOfGoodsSold.ToString("N0", CultureInfo.CurrentCulture),
                (flip.GrossProfit * flip.Volume).ToString("N0", CultureInfo.CurrentCulture),
                flip.ProfitMargin.ToString("P2", CultureInfo.CurrentCulture));
        }

        AnsiConsole.Write(table);
    }

    private Station? PromptUserForTradeHub()
    {
        var prompt = new SelectionPrompt<string>()
            .Title("Select a trade hub:")
            .AddChoices([
                "Jita IV - Moon 4 - Caldari Navy Assembly Plant",
                "Amarr VIII (Oris) - Emperor Family Academy",
                "Rens VI - Moon 8 - Brutor Tribe Treasury",
                "Dodixie IX - Moon 20 - Federation Navy Assembly Plant",
                "Hek VIII - Moon 12 - Boundless Creation Factory"]);

        var tradeHubChoice = AnsiConsole.Prompt(prompt);

        return context.Stations
            .Include(station => station.SolarSystem)
            .ThenInclude(station => station.Region)
            .FirstOrDefault(station => station.Name == tradeHubChoice);
    }

    private static void PrintPrettyStationName(Station station)
    {
        var colour = station.SolarSystem.Region.Name switch
        {
            "Domain" => "gold1",
            "The Forge" => "deepskyblue2",
            "Metropolis" or "Heimatar" => "red3",
            "Sinq Laison" => "turquoise4",
            _ => ""
        };

        AnsiConsole.MarkupLine($"[{colour}]{station.Name}[/]");
    }
}
