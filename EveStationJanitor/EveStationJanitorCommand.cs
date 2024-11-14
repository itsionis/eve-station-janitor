using System.CommandLine;
using System.CommandLine.Parsing;

namespace EveStationJanitor;

public class EveStationJanitorCommand : RootCommand
{
    public EveStationJanitorCommand(Func<string?, TradeHubStation?, int, Task> handler)
    {
        var optionCharacter = new Option<string?>("--character");
        AddOption(optionCharacter);

        var optionTradeHubSystem = new OptionTradeHubSystem("--trade-hub");
        AddOption(optionTradeHubSystem);

        var optionMinimumProfit = new OptionMinimumProfit("--min-profit");
        optionMinimumProfit.SetDefaultValue(0);
        AddOption(optionMinimumProfit);

        this.SetHandler(handler, optionCharacter, optionTradeHubSystem, optionMinimumProfit);
    }
}

public class OptionMinimumProfit(string name) : Option<int>(name)
{
    public override string Description => "Ignore item flips which have a projected profit less than this amount in ISK";
}

public class OptionTradeHubSystem(string name)
    : Option<TradeHubStation?>(name, parseArgument: ParseTradeHubSystemArgument)
{
    public override string Description => "The trade hub system where market opportunities are located";

    private static TradeHubStation? ParseTradeHubSystemArgument(ArgumentResult result)
    {
        if (!result.Tokens.Any())
        {
            return null;
        }

        var token = result.Tokens.SingleOrDefault()?.Value;
        return TradeHubStation.TryGetStationBySystem(token);
    }
}