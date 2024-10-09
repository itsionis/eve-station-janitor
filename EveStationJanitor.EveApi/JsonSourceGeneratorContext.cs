using EveStationJanitor.EveApi.Character.Objects;
using EveStationJanitor.EveApi.Market.Objects;
using EveStationJanitor.EveApi.Universe.Objects;
using System.Text.Json.Serialization;

namespace EveStationJanitor.EveApi;

[JsonSerializable(typeof(ApiItemType))]
[JsonSerializable(typeof(ApiItemGroup))]
[JsonSerializable(typeof(ApiItemCategory))]
[JsonSerializable(typeof(ApiCharacterSkill))]
[JsonSerializable(typeof(ApiCharacterSkills))]
[JsonSerializable(typeof(List<ApiCharacterStanding>))]
[JsonSerializable(typeof(List<ApiMarketOrder>))]
[JsonSerializable(typeof(ApiStandingType))]
internal partial class JsonSourceGeneratorContext : JsonSerializerContext
{
    public const string ServiceKey = "api-json-serializer";
}
