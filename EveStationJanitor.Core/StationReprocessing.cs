using EveStationJanitor.Core.DataAccess.Entities;
using EveStationJanitor.Core.Eve;
using EveStationJanitor.Core.Eve.Formula;

namespace EveStationJanitor.Core;

public class StationReprocessing : IReprocessingFacility
{
    private readonly OreReprocessing _oreReprocessing;
    private readonly Skills _skills;
    private readonly CloneImplants _implants;
    private readonly decimal _stationBaseYield;

    public StationReprocessing(OreReprocessing oreReprocessing, Station station, Skills skills, Standings standings, CloneImplants implants)
    {
        _oreReprocessing = oreReprocessing;
        _stationBaseYield = (decimal)station.ReprocessingEfficiency;
        _skills = skills;
        _implants = implants;

        var stationOwnerStandings = standings.GetEffectiveStanding(station.OwnerCorporationId);
        ReprocessingTaxPercent = TaxFormula.StationReprocessingEquipmentTax(station.ReprocessingTax, stationOwnerStandings);
    }
    
    public decimal ReprocessingTaxPercent { get; }

    public long ReprocessedMaterialQuantity(ItemType itemBeingReprocessed, int baseReprocessedQuantity)
    {
        const int asteroidCategoryId = 25;
        decimal yieldPercent;

        if (itemBeingReprocessed.Group.Category.Id == asteroidCategoryId)
        {
            var oreReprocessingSkill = _oreReprocessing.GetOreReprocessingSkillLevel(itemBeingReprocessed.Id);
            yieldPercent = ReprocessingFormula.StationOreYield(_stationBaseYield, _skills.Reprocessing, _skills.ReprocessingEfficiency, oreReprocessingSkill, _implants.ImplantReprocessingEfficiency);
        }
        else
        {
            yieldPercent = ReprocessingFormula.ScrapMetalYield(_stationBaseYield, _skills.ScrapmetalProcessing);
        }

        return (long)Math.Truncate(baseReprocessedQuantity * yieldPercent);
    }
}
