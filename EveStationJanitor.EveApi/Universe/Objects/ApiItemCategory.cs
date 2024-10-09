using System.Text.Json.Serialization;

namespace EveStationJanitor.EveApi.Universe.Objects;

public class ApiItemCategory
{
    [JsonPropertyName("category_id")]
    public int Id { get; set; }


    [JsonPropertyName("name")]
    public required string Name { get; set; }
}
