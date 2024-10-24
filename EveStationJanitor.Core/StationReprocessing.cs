using EveStationJanitor.Core.DataAccess.Entities;
using System.Collections.Frozen;

namespace EveStationJanitor.Core;

public class StationReprocessing
{
    private static readonly FrozenDictionary<int, decimal> _implantReprocessingBonuses = new Dictionary<int, decimal> {
        { 27175, 0.01m }, // Zainou 'Beancounter' Reprocessing RX-801
        { 27169, 0.02m }, // Zainou 'Beancounter' Reprocessing RX-802
        { 27174, 0.04m }  // Zainou 'Beancounter' Reprocessing RX-804
    }.ToFrozenDictionary();

    private readonly OreReprocessing _oreReprocessing;
    private readonly Station _station;
    private readonly Skills _skills;

    private readonly decimal _implantReprocessingEfficiency = 0.0m;
    private readonly Dictionary<int, decimal> _yieldEfficiencyCache = [];

    public StationReprocessing(OreReprocessing oreReprocessing, Station station, Skills skills, Standings standings, CloneImplants implants)
    {
        _oreReprocessing = oreReprocessing;
        _station = station;
        _skills = skills;
        _implantReprocessingEfficiency = CalculateCloneImplantBonusReprocessingEfficiency(implants);
        StationReprocessingTaxPercent = GetStationReprocessingTax(standings, station);
    }

    private static decimal CalculateCloneImplantBonusReprocessingEfficiency(CloneImplants implants)
    {
        if (implants.Implants.Count == 0)
        {
            return 0.0m;
        }

        foreach (var implant in implants.Implants)
        {
            // This assumes there's only one reprocessing implant pluggable at any one time...
            if (_implantReprocessingBonuses.TryGetValue(implant.Id, out var bonus))
            {
                return bonus;
            }
        }

        return 0.0m;
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
                    (1 + _implantReprocessingEfficiency);

                return _yieldEfficiencyCache[itemBeingReprocessed.Id] = oreReprocessingEfficiency;
            }
        }
        else
        {
            // Calculate scrapmetal reprocessing yield efficiency
            var scrapMetalReprocessingEfficiency =
                (decimal)_station.ReprocessingEfficiency *
                (1 + _skills.ScrapMetalProcessing * 0.02m) *
                (1 + _implantReprocessingEfficiency);

            return _yieldEfficiencyCache[itemBeingReprocessed.Id] = scrapMetalReprocessingEfficiency;
        }
    }
}
