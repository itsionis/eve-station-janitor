using EveStationJanitor.Core.DataAccess;
using EveStationJanitor.Core;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using EveStationJanitor.Core.DataAccess.Entities;
using System.Globalization;
using EveStationJanitor.Authentication;

namespace EveStationJanitor;

internal sealed class Janitor(
    AppDbContext context,
    IEveCharacterDataProvider eveCharacterDataProvider,
    IEveMarketOrdersRepository marketOrdersRepository,
    IAuthenticationDataProvider authenticationDataProvider)
{
    public async Task Run(string? characterName = null, TradeHubStation? tradeHubStationChoice = null)
    {
        var characterId = await GetCharacterId(characterName);
        if (characterId is null)
        {
            return;
        }

        var station = GetTradeHubStation(tradeHubStationChoice);
        if (station is null)
        {
            return;
        }

        PrintPrettyStationName(station);

        var characterData = eveCharacterDataProvider.CreateForCharacter(characterId.Value);

        var implantsResult = await characterData.GetActiveCloneImplants();
        if (!implantsResult.TryPickT0(out var implants, out var implantError))
        {
            AnsiConsole.Markup($"[red]!! Error fetching implants[/] {implantError.Value}");
            return;
        }

        var skillsResult = await characterData.GetSkills();
        if (!skillsResult.TryPickT0(out var skills, out var skillsError))
        {
            AnsiConsole.Markup($"[red]!! Error fetching skills[/] {skillsError.Value}");
            return;
        }

        var standingsResult = await characterData.GetStandings(skills);
        if (!standingsResult.TryPickT0(out var standings, out var standingsError))
        {
            AnsiConsole.Markup($"[red]!! Error fetching standings[/] {standingsError.Value}");
            return;
        }

        // Load regional market orders
        await marketOrdersRepository.LoadOrders(station);

        var oreReprocessing = new OreReprocessing(skills);
        var stationReprocessing = new StationReprocessing(oreReprocessing, station, skills, standings, implants);

        var profitCalculator = new ProfitCalculator(marketOrdersRepository, stationReprocessing, skills);
        var flips = await profitCalculator.FindMostProfitableOrders();

        WriteResultsTable(flips);
    }

    private Station? GetTradeHubStation(TradeHubStation? tradeHubSystem)
    {
        if (tradeHubSystem is null)
        {
            return PromptUserForTradeHub();
        }
            
        return context.Stations
            .Include(station => station.SolarSystem)
            .ThenInclude(station => station.Region)
            .FirstOrDefault(station => station.Name == tradeHubSystem.StationName);
    }

    private static void WriteResultsTable(IReadOnlyList<ItemFlipAppraisal> flips)
    {
        var table = new Table();

        table.AddColumn("Item");

        table.AddColumn("Quantity");
        table.Columns[1].RightAligned();

        table.AddColumn("Cost per");
        table.Columns[2].RightAligned();

        table.AddColumn("Total cost");
        table.Columns[3].RightAligned();

        table.AddColumn("Total profit");
        table.Columns[4].RightAligned();

        table.AddColumn("Margin");
        table.Columns[5].RightAligned();

        table.AddColumn("Score");
        table.Columns[6].RightAligned();

        foreach (var flip in flips.OrderByDescending(flip => flip.Score))
        {
            table.AddRow(
                flip.Item.Name,
                flip.QuantityToBuy.ToString("N0", CultureInfo.CurrentCulture),
                (flip.CostOfGoodsSold / flip.QuantityToBuy).ToString("N2", CultureInfo.CurrentCulture),
                flip.CostOfGoodsSold.ToString("N2", CultureInfo.CurrentCulture),
                flip.GrossProfit.ToString("N2", CultureInfo.CurrentCulture),
                flip.ProfitMargin.ToString("P2", CultureInfo.CurrentCulture),
                flip.Score.ToString("N2", CultureInfo.CurrentCulture));
        }
        
        AnsiConsole.Write(table);
    }

    private async Task<int?> GetCharacterId(string? characterName)
    {
        var prompter = new EveCharacterSelectionLogic(context, authenticationDataProvider);
        if (characterName is null)
        {
            return await prompter.PromptForCharacterId();
        }
        
        // If the character is found, then we need to make sure it's authenticated. When a character is picked from
        // the list this occurs automatically.
        var maybeCharacter = await context.Characters.FirstOrDefaultAsync(character => character.Name == characterName);
        if (maybeCharacter is not null)
        {
            var isAuthenticated = await authenticationDataProvider.EnsureCharacterAuthenticated(maybeCharacter.EveCharacterId);
            if (isAuthenticated)
            {
                return maybeCharacter.EveCharacterId;
            }
            
            AnsiConsole.MarkupLine(" {0} is not authenticated. Sign in as this character or pick a new one.", characterName);
            return await prompter.PromptForCharacterId();
        }
        
        // Character not found, prompt them for one...
        AnsiConsole.MarkupLine("Could not find character {0}", characterName);
        return await prompter.PromptForCharacterId();
    }

    private Station? PromptUserForTradeHub()
    {
        var prompt = new SelectionPrompt<string>()
            .Title("Select a trade hub:")
            .AddChoices([
                TradeHubStation.Jita.StationName,
                TradeHubStation.Amarr.StationName,
                TradeHubStation.Rens.StationName,
                TradeHubStation.Dodixie.StationName,
                TradeHubStation.Hek.StationName
            ]);

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
