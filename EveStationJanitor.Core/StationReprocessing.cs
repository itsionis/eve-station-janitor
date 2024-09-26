using EveStationJanitor.Core.DataAccess.Entities;

namespace EveStationJanitor.Core;

public class StationReprocessing
{
    private readonly OreReprocessing _oreReprocessing;
    private readonly Station _station;
    private readonly Skills _skills;

    private readonly Dictionary<int, decimal> _yieldEfficiencyCache = [];

    public StationReprocessing(OreReprocessing oreReprocessing, Station station, Skills skills, Standings standings)
    {
        _oreReprocessing = oreReprocessing;
        _station = station;
        _skills = skills;
        StationReprocessingTaxPercent = GetStationReprocessingTax(standings, station);
    }

    public Station Station => _station;

    public decimal StationReprocessingTaxPercent { get; }

    private static decimal GetStationReprocessingTax(Standings standings, Station station)
    {
        var standing = standings.GetStanding(station.OwnerCorporationId);
        var scalingFactor = station.ReprocessingTax / 6.67d;
        var scaledStanding = Math.Clamp(standing, 0d, 6.67d);
        return (decimal)(scaledStanding * scalingFactor);
    }

    public decimal CalculateYieldEfficiency(ItemType itemBeingReprocessed)
    {
        if (_yieldEfficiencyCache.TryGetValue(itemBeingReprocessed.Id, out var efficiency))
        {
            return efficiency;
        }

        // Get skill levels from the CharacterSkills class

        if (itemBeingReprocessed.Group.Category.Name == "Asteroid")
        {
            // Calculate asteroid reprocessing yield efficiency
            var oreReprocessingSkillLevel = _oreReprocessing.GetOreReprocessingSkillLevel(itemBeingReprocessed.Id);
            if (oreReprocessingSkillLevel is null)
            {
                return _yieldEfficiencyCache[itemBeingReprocessed.Id] = (decimal)_station.ReprocessingEfficiency;
            }
            else
            {
                var oreReprocessingEfficiency =
                    (decimal)_station.ReprocessingEfficiency *
                    (1 + _skills.Reprocessing * 0.03m) *
                    (1 + _skills.ReprocessingEfficiency * 0.02m) *
                    (1 + oreReprocessingSkillLevel.Value * 0.02m) *
                    (1 + 0); // TODO: IMPLANT

                return _yieldEfficiencyCache[itemBeingReprocessed.Id] = oreReprocessingEfficiency;
            }
        }
        else
        {
            // Calculate scrapmetal reprocessing yield efficiency
            var scrapMetalReprocessingEfficiency =
                (decimal)_station.ReprocessingEfficiency *
                (1 + _skills.ScrapMetalProcessing * 0.02m) *
                (1 + 0m); // TODO: IMPLANTS

            return _yieldEfficiencyCache[itemBeingReprocessed.Id] = scrapMetalReprocessingEfficiency;
        }
    }
}