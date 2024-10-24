namespace EveStationJanitor.Core.Eve.Formula;

public static class ReprocessingFormula
{
    /// <summary>
    /// The yield percent when reprocessing items applicable to the scrapmetal reprocessing skill.
    /// </summary>
    /// <remarks>
    /// Calculation derived from https://wiki.eveuniversity.org/Reprocessing#Scrapmetal_reprocessing
    /// </remarks>
    public static decimal ScrapMetalYield(decimal stationBaseYield, int scrapMetalProcessingSkill)
    {
        scrapMetalProcessingSkill = Math.Clamp(scrapMetalProcessingSkill, 0, 5);
        return stationBaseYield * (1 + scrapMetalProcessingSkill * 0.02m);
    }

    /// <summary>
    /// The yield percent when reprocessing ores in an NPC station.
    /// </summary>
    /// <remarks>
    /// Calculation derived from https://wiki.eveuniversity.org/Reprocessing#Reprocessing_in_a_station
    /// </remarks>
    public static decimal StationOreYield(
        decimal stationBaseYield,
        int reprocessingSkill,
        int reprocessingEfficiencySkill,
        int oreReprocessingSkill,
        decimal implantReprocessingBonus)
    {
        reprocessingSkill = Math.Clamp(reprocessingSkill, 0, 5);
        oreReprocessingSkill = Math.Clamp(oreReprocessingSkill, 0, 5);
        reprocessingEfficiencySkill = Math.Clamp(reprocessingEfficiencySkill, 0, 5);

        return stationBaseYield *
            (1 + reprocessingSkill * 0.03m) *
            (1 + reprocessingEfficiencySkill * 0.02m) *
            (1 + oreReprocessingSkill * 0.02m) *
            (1 + implantReprocessingBonus);
    }
}
