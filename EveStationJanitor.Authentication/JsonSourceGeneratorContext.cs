using EveStationJanitor.Authentication.Tokens;
using System.Text.Json.Serialization;

namespace EveStationJanitor.Authentication;

[JsonSerializable(typeof(EveSsoTokens))]
[JsonSerializable(typeof(AuthorizedToken))]
internal partial class JsonSourceGeneratorContext : JsonSerializerContext
{
    public const string ServiceKey = "auth-json-serializer";
}
