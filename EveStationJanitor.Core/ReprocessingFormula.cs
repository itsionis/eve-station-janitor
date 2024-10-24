namespace EveStationJanitor.Core;

public static class ReprocessingFormula
{
    public static decimal ScrapMetalYield(decimal stationBaseYield, int scrapMetalProcessingSkill)
    {
        scrapMetalProcessingSkill = Math.Clamp(scrapMetalProcessingSkill, 0, 5);
        
        // Calculation derived from https://wiki.eveuniversity.org/Reprocessing#Scrapmetal_reprocessing
        return stationBaseYield * (1 + (scrapMetalProcessingSkill * 0.02m));
    }

    public static decimal OreYield(
        decimal stationBaseYield,
        int reprocessingSkill,
        int reprocessingEfficiencySkill,
        int oreReprocessingSkill,
        decimal implantReprocessingBonus)
    {
        reprocessingSkill = Math.Clamp(reprocessingSkill, 0, 5);
        oreReprocessingSkill = Math.Clamp(oreReprocessingSkill, 0, 5);
        reprocessingEfficiencySkill = Math.Clamp(reprocessingEfficiencySkill, 0, 5);

        // NB. This formula assumes the structure is an NPC station and not an Upwell structure.
        // Calculation derived from https://wiki.eveuniversity.org/Reprocessing#Reprocessing_in_a_station
        return stationBaseYield *
            (1 + (reprocessingSkill * 0.03m)) *
            (1 + (reprocessingEfficiencySkill * 0.02m)) *
            (1 + (oreReprocessingSkill * 0.02m)) *
            (1 + implantReprocessingBonus);
    }
}
