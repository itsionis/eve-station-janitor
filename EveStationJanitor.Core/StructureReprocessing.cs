using EveStationJanitor.Core.DataAccess.Entities;
using EveStationJanitor.Core.Eve;
using EveStationJanitor.Core.Eve.Formula;

namespace EveStationJanitor.Core;

public class StructureReprocessing : IReprocessingFacility
{
    /// <summary>
    /// All structures have a base yield of 50% for scrapmetal reprocessing. This is not affected by the structure or rig bonuses.
    /// </summary>
    private const decimal StructureBaseScrapMetalYield = 0.5m;
    
    private readonly OreReprocessing _oreReprocessing;
    private readonly Structure _structure;
    private readonly Skills _skills;
    private readonly CloneImplants _implants;

    public StructureReprocessing(OreReprocessing oreReprocessing, Structure structure, Skills skills, CloneImplants implants)
    {
        _oreReprocessing = oreReprocessing;
        _structure = structure;
        _skills = skills;
        _implants = implants;
    }
    
    public decimal ReprocessingTaxPercent => _structure.ReprocessingTax;

    public long ReprocessedMaterialQuantity(ItemType itemBeingReprocessed, int baseReprocessedQuantity)
    {
        const int asteroidCategoryId = 25;
        decimal yieldPercent;

        if (itemBeingReprocessed.Group.Category.Id == asteroidCategoryId)
        {
            var oreReprocessingSkill = _oreReprocessing.GetOreReprocessingSkillLevel(itemBeingReprocessed.Id);
            yieldPercent = ReprocessingFormula.StructureOreYield(_structure.ReprocessingRigLevel, _structure.SecurityType, _structure.Type, _skills.Reprocessing, _skills.ReprocessingEfficiency, oreReprocessingSkill, _implants.ImplantReprocessingEfficiency);
        }
        else
        {
            yieldPercent = ReprocessingFormula.ScrapMetalYield(StructureBaseScrapMetalYield, _skills.ScrapmetalProcessing);
        }

        return (long)Math.Truncate(baseReprocessedQuantity * yieldPercent);
    }
}
