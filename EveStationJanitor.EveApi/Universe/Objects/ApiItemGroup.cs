using System.Text.Json.Serialization;

namespace EveStationJanitor.EveApi.Universe.Objects;

public class ApiItemGroup
{
    [JsonPropertyName("group_id")]
    public required int Id { get; set; }

    [JsonPropertyName("category_id")]
    public required int CategoryId { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }
}
