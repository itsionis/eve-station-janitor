using System.CommandLine;
using System.CommandLine.Parsing;

namespace EveStationJanitor;

public class EveStationJanitorCommand : RootCommand
{
    public EveStationJanitorCommand(Func<string?, TradeHubStation?, Task> handler)
    {
        var optionCharacter = new Option<string?>("--character");
        AddOption(optionCharacter);

        var optionTradeHubSystem = new OptionTradeHubSystem("--trade-hub");
        AddOption(optionTradeHubSystem);

        this.SetHandler(handler, optionCharacter, optionTradeHubSystem);
    }
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