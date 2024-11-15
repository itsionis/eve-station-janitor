using EveStationJanitor.EveApi.Character.Objects;
using EveStationJanitor.EveApi.Market.Objects;
using EveStationJanitor.EveApi.Universe.Objects;
using System.Text.Json.Serialization;
using EveStationJanitor.EveApi.Status.Objects;

namespace EveStationJanitor.EveApi;

[JsonSerializable(typeof(ApiItemType))]
[JsonSerializable(typeof(ApiItemGroup))]
[JsonSerializable(typeof(ApiItemCategory))]
[JsonSerializable(typeof(ApiCharacterSkill))]
[JsonSerializable(typeof(ApiCharacterSkills))]
[JsonSerializable(typeof(ApiEveServerStatus))]
[JsonSerializable(typeof(List<ApiCharacterStanding>))]
[JsonSerializable(typeof(List<ApiMarketOrder>))]
[JsonSerializable(typeof(List<int>))]
[JsonSerializable(typeof(ApiStandingType))]
[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
internal partial class JsonSourceGeneratorContext : JsonSerializerContext;
