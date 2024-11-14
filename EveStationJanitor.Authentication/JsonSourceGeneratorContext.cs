using System.Text.Json.Serialization;
using EveStationJanitor.Authentication.Validation;

namespace EveStationJanitor.Authentication;

[JsonSerializable(typeof(EveSsoTokens))]
[JsonSerializable(typeof(AuthorizedToken))]
[JsonSerializable(typeof(List<int>))]
[JsonSerializable(typeof(List<TokenValidator.CharacterAffiliations>))]
internal partial class JsonSourceGeneratorContext : JsonSerializerContext;
