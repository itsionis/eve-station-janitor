namespace EveStationJanitor;

public class TradeHubStation
{
    private static readonly Dictionary<string, TradeHubStation> StationsBySystemName = new();

    public static readonly TradeHubStation Jita = new("Jita", "Jita IV - Moon 4 - Caldari Navy Assembly Plant");
    public static readonly TradeHubStation Amarr = new("Amarr", "Amarr VIII (Oris) - Emperor Family Academy");
    public static readonly TradeHubStation Dodixie = new("Dodixie","Dodixie IX - Moon 20 - Federation Navy Assembly Plant");
    public static readonly TradeHubStation Hek = new("Hek", "Hek VIII - Moon 12 - Boundless Creation Factory");
    public static readonly TradeHubStation Rens = new("Rens", "Rens VI - Moon 8 - Brutor Tribe Treasury");
    
    public string SystemName { get; }
    
    public string StationName { get; }
    
    private TradeHubStation(string systemName, string stationName)
    {
        SystemName = systemName;
        StationName = stationName;
        RegisterStation(systemName);
    }

    private void RegisterStation(string systemName)
    {
        StationsBySystemName[systemName.ToLowerInvariant()] = this;
    }

    public static TradeHubStation? TryGetStationBySystem(string? systemName)
    {
        if (systemName is null) return null;
        StationsBySystemName.TryGetValue(systemName.ToLowerInvariant(), out var system);
        return system;
    }
}
