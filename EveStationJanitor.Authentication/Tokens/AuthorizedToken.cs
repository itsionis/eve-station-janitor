using NodaTime;

namespace EveStationJanitor.Authentication.Tokens;

public sealed class AuthorizedToken
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
    public required Instant ExpiresOn { get; set; }
    public required string CharacterName { get; set; }
    public required int CharacterId { get; set; }
    public required int CorporationId { get; set; }
    public required int? AllianceId { get; set; }
    public required int? FactionId { get; set; }
    public required HashSet<string> Scopes { get; set; } = [];
}
