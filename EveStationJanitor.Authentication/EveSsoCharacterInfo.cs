namespace EveStationJanitor.Authentication;

public class EveSsoCharacterInfo
{
    public required string CharacterName { get; set; }
    public required string CharacterOwnerHash { get; set; }
    public required int CharacterId { get; set; }
    public required int CorporationId { get; set; }
    public required int? AllianceId { get; set; }
    public required int? FactionId { get; set; }
}
