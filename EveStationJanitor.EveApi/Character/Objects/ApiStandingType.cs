using System.Text.Json;
using System.Text.Json.Serialization;

namespace EveStationJanitor.EveApi.Character.Objects;

[JsonConverter(typeof(ApiStandingTypeConverter))]
public enum ApiStandingType
{
    Agent,
    NpcCorp,
    Faction
}

internal class ApiStandingTypeConverter : JsonConverter<ApiStandingType>
{
    public override ApiStandingType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return value switch
        {
            "agent" => ApiStandingType.Agent,
            "npc_corp" => ApiStandingType.NpcCorp,
            "faction" => ApiStandingType.Faction,
            _ => throw new JsonException($"Value '{value}' is not valid for ApiStandingType")
        };
    }

    public override void Write(Utf8JsonWriter writer, ApiStandingType value, JsonSerializerOptions options)
    {
        var stringValue = value switch
        {
            ApiStandingType.Agent => "agent",
            ApiStandingType.NpcCorp => "npc_corp",
            ApiStandingType.Faction => "faction",
            _ => throw new JsonException($"Value '{value}' is not valid for ApiStandingType")
        };
        writer.WriteStringValue(stringValue);
    }
}
