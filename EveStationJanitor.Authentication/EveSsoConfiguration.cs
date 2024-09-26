using System.ComponentModel.DataAnnotations;

namespace EveStationJanitor.Authentication;

public sealed class EveSsoConfiguration
{
    public const string ConfigurationSectionName = "EveSso";

    [Required]
    public required string ClientId { get; set; }

    [Required]
    public required string CallbackUrl { get; set; }
    
    public TimeSpan AuthenticationTimeout { get; set; } = TimeSpan.FromMinutes(2);
}
