using System.Text.Json.Serialization;

namespace EveStationJanitor.EveApi.Character.Objects;

[JsonConverter(typeof(JsonStringEnumConverter<ApiStandingType>))]
public enum ApiStandingType
{
    agent,
    npc_corp,
    faction
}