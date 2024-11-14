using System.Text.Json.Serialization;

namespace EveStationJanitor.EveApi.Universe.Objects;

public class ApiItemType
{
    [JsonPropertyName("type_id")]
    public required int TypeId { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("group_id")]
    public required int GroupId { get; set; }

    [JsonPropertyName("volume")]
    public decimal Volume { get; set; }

    [JsonPropertyName("mass")]
    public decimal Mass { get; set; }

    [JsonPropertyName("portion_size")]
    public int PortionSize { get; set; }
}
