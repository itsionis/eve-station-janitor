using System.Text.Json.Serialization;

namespace EveStationJanitor.EveApi.Character.Objects;

public class ApiCharacterStanding
{
    [JsonPropertyName("from_id")]
    public int FromId { get; set; }

    [JsonPropertyName("from_type")]
    public ApiStandingType FromType { get; set; }

    [JsonPropertyName("standing")]
    public double Standing { get; set; }
}
