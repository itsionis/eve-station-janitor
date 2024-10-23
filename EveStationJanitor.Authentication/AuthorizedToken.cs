using NodaTime;

namespace EveStationJanitor.Authentication;

public sealed class AuthorizedToken
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
    public required Instant ExpiresOn { get; set; }
    public required HashSet<string> Scopes { get; set; } = [];
}
