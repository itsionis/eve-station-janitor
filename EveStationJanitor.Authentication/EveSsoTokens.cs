using System.Text.Json.Serialization;

namespace EveStationJanitor.Authentication;

public class EveSsoTokens
{
    [JsonPropertyName("access_token")]
    public required string AccessToken { get; set; }

    [JsonPropertyName("token_type")]
    public required string TokenType { get; set; }

    [JsonPropertyName("expires_in")]
    public required int ExpiresIn { get; set; }

    [JsonPropertyName("refresh_token")]
    public required string RefreshToken { get; set; }
}
