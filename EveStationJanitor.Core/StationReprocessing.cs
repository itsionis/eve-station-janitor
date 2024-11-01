using EveStationJanitor.Core.DataAccess.Entities;
using EveStationJanitor.Core.Eve;
using EveStationJanitor.Core.Eve.Formula;
using System.Collections.Frozen;

namespace EveStationJanitor.Core;

public class StationReprocessing
{
    private readonly OreReprocessing _oreReprocessing;
    private readonly Station _station;
    private readonly Skills _skills;
    private readonly CloneImplants _implants;
    private readonly decimal _stationBaseYield;

    public StationReprocessing(OreReprocessing oreReprocessing, Station station, Skills skills, Standings standings, CloneImplants implants)
    {
        _oreReprocessing = oreReprocessing;
        _station = station;
        _stationBaseYield = (decimal)_station.ReprocessingEfficiency;
        _skills = skills;
        _implants = implants;

        var stationOwnerStandings = standings.GetStanding(station.OwnerCorporationId);
        StationReprocessingTaxPercent = TaxFormula.StationReprocessingEquipmentTax(station.ReprocessingTax, stationOwnerStandings);
    }
    
    public Station Station => _station;

    public decimal StationReprocessingTaxPercent { get; }

    public long ReprocessedMaterialQuantity(ItemTypeMaterial material)
    {
        var itemBeingReprocessed = material.ItemType;

        const int asteroidCategoryId = 25;
        if (itemBeingReprocessed.Group.Category.Id == asteroidCategoryId)
        {
            var oreReprocessingSkill = _oreReprocessing.GetOreReprocessingSkillLevel(itemBeingReprocessed.Id);
            var quantityWithBonusYield = material.Quantity;
            var yieldPercent = ReprocessingFormula.StationOreYield(_stationBaseYield, _skills.Reprocessing, _skills.ReprocessingEfficiency, oreReprocessingSkill, _implants.ImplantReprocessingEfficiency);
            return (long)Math.Truncate(quantityWithBonusYield * yieldPercent);
        }
        else
        {
            var yieldPercent = ReprocessingFormula.ScrapMetalYield(_stationBaseYield, _skills.ScrapmetalProcessing);
            return (long)Math.Truncate(material.Quantity * yieldPercent);
        }
    }
}
