namespace EveStationJanitor.Core.Eve.Formula;

public static class ReprocessingFormula
{
    /// <summary>
    /// The yield percent when reprocessing items applicable to the scrapmetal reprocessing skill. Yield is not affected
    /// by player structure bonuses.
    /// </summary>
    /// <remarks>
    /// Calculation derived from <see href="https://wiki.eveuniversity.org/Reprocessing#Scrapmetal_reprocessing"/>
    /// </remarks>
    public static decimal ScrapMetalYield(decimal stationBaseYield, SkillLevel scrapMetalProcessingSkill)
    {
        scrapMetalProcessingSkill = Math.Clamp(scrapMetalProcessingSkill, 0, 5);
        return stationBaseYield * (1 + (scrapMetalProcessingSkill * 0.02m));
    }

    /// <summary>
    /// The yield percent when reprocessing ores in an NPC station.
    /// </summary>
    /// <remarks>
    /// Calculation derived from <see href="https://wiki.eveuniversity.org/Reprocessing#Reprocessing_in_a_station"/>
    /// </remarks>
    public static decimal StationOreYield(
        decimal stationBaseYield,
        SkillLevel reprocessingSkill,
        SkillLevel reprocessingEfficiencySkill,
        SkillLevel oreReprocessingSkill,
        decimal implantReprocessingBonus)
    {
        return stationBaseYield * CharacterOreYield(reprocessingSkill, reprocessingEfficiencySkill, oreReprocessingSkill, implantReprocessingBonus);
    }

    /// <summary>
    /// The yield percent when reprocessing ores in a player Upwell structure.
    /// </summary>
    /// <remarks>
    /// Calculation derived from
    /// <see href="https://forums.eveonline.com/t/what-is-the-new-ore-refining-formula-for-athanor-and-tatara-citadels/38090/25"/>
    /// and <see href="https://wiki.eveuniversity.org/Reprocessing#Upwell_reprocessing_formula"/>
    /// </remarks>
    public static decimal StructureOreYield(
        StructureReprocessingRigLevel structureRigLevel,
        SpaceSecurityType spaceSecurityType,
        StructureType structureType,
        SkillLevel reprocessingSkill,
        SkillLevel reprocessingEfficiencySkill,
        SkillLevel oreReprocessingSkill,
        decimal implantReprocessingBonus)
    {
        // TODO - Some rigs provide bonuses to specific ore types. E.g. "Standup M-Set Moon Ore Grading Processor I" will not provide bonuses to reprocessing asteroid ores. 
        var structureRigLevelModifier = structureRigLevel switch
        {
            StructureReprocessingRigLevel.None => 0.0m,
            StructureReprocessingRigLevel.Tech1 => 0.01m,
            StructureReprocessingRigLevel.Tech2 => 0.03m,
            _ => throw new ArgumentOutOfRangeException(nameof(spaceSecurityType), spaceSecurityType, "Structure reprocessing rig not supported.")
        };
        
        // Only refineries provide reprocessing bonuses
        var structureTypeModifier = structureType switch
        {
            StructureType.Athanor => 0.02m,
            StructureType.Tatara => 0.055m,
            _ => 0.0m
        };
        
        // System security modifier only applies when a rig is installed.
        var spaceSecurityTypeModifier = 0.0m;
        if (structureRigLevel != StructureReprocessingRigLevel.None)
        {
            spaceSecurityTypeModifier = spaceSecurityType switch
            {
                SpaceSecurityType.HighSec => 0.0m,
                SpaceSecurityType.LowSec => 0.06m,
                SpaceSecurityType.NullSec => 0.12m,
                SpaceSecurityType.Wormhole => 0.12m,
                _ => throw new ArgumentOutOfRangeException(nameof(spaceSecurityType), spaceSecurityType, "Space security type not supported.")
            };
        }
        
        var structureBaseYield = 
            (0.5m + structureRigLevelModifier) * 
            (1 + spaceSecurityTypeModifier) *
            (1 + structureTypeModifier);
        
        return structureBaseYield * CharacterOreYield(reprocessingSkill, reprocessingEfficiencySkill, oreReprocessingSkill, implantReprocessingBonus);
    }

    private static decimal CharacterOreYield(
        SkillLevel reprocessingSkill,
        SkillLevel reprocessingEfficiencySkill,
        SkillLevel oreTypeReprocessingSkill,
        decimal implantReprocessingBonus)
    {
        reprocessingSkill = Math.Clamp(reprocessingSkill, 0, 5);
        oreTypeReprocessingSkill = Math.Clamp(oreTypeReprocessingSkill, 0, 5);
        reprocessingEfficiencySkill = Math.Clamp(reprocessingEfficiencySkill, 0, 5);
        
        return (1 + (reprocessingSkill * 0.03m)) *
               (1 + (reprocessingEfficiencySkill * 0.02m)) *
               (1 + (oreTypeReprocessingSkill * 0.02m)) *
               (1 + implantReprocessingBonus);
    }
}
