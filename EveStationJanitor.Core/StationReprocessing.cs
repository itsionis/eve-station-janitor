﻿using EveStationJanitor.Core.DataAccess.Entities;
using EveStationJanitor.Core.Eve;
using EveStationJanitor.Core.Eve.Formula;
using System.Collections.Frozen;
using System.Diagnostics;

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
    private readonly decimal _stationBaseYield;
    private readonly decimal _implantReprocessingEfficiency = 0.0m;

    public StationReprocessing(OreReprocessing oreReprocessing, Station station, Skills skills, Standings standings, CloneImplants implants)
    {
        _oreReprocessing = oreReprocessing;
        _station = station;
        _stationBaseYield = (decimal)_station.ReprocessingEfficiency;
        _skills = skills;
        _implantReprocessingEfficiency = CalculateCloneImplantBonusReprocessingEfficiency(implants);

        var stationOwnerStandings = standings.GetStanding(station.OwnerCorporationId);
        StationReprocessingTaxPercent = TaxFormula.StationReprocessingEquipmentTax(station.ReprocessingTax, stationOwnerStandings);
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

    public decimal CalculateReprocessedMaterialQuantity(ItemTypeMaterial material)
    {
        var itemBeingReprocessed = material.ItemType;

        if (itemBeingReprocessed.Group.Category.Name == "Asteroid")
        {
            var oreReprocessingSkill = _oreReprocessing.GetOreReprocessingSkillLevel(itemBeingReprocessed.Id);
            var quantityWithBonusYield = material.Quantity * (1 + _oreReprocessing.GetOreBonusYield(itemBeingReprocessed.Id));
            var yieldPercent = ReprocessingFormula.StationOreYield(_stationBaseYield, _skills.Reprocessing, _skills.ReprocessingEfficiency, oreReprocessingSkill.GetValueOrDefault(0), _implantReprocessingEfficiency);
            return Math.Truncate(quantityWithBonusYield * yieldPercent);
        }
        else
        {
            var yieldPercent = ReprocessingFormula.ScrapMetalYield(_stationBaseYield, _skills.ScrapMetalProcessing);
            return Math.Truncate(material.Quantity * yieldPercent);
        }
    }
}
