namespace EveStationJanitor.Core.Eve.Formula;

public static class TaxFormula
{
    /// <summary>
    /// Paid when reprocessing items in an NPC station. Affected by standings, this is a tax on the estimated value of
    /// the results of the reprocessing.
    /// </summary>
    /// <remarks>
    /// Calculation derived from https://wiki.eveuniversity.org/Tax#Equipment_tax
    /// </remarks>
    public static Tax StationReprocessingEquipmentTax(Tax baseReprocessingTax, Standing stationOwnerStandings)
    {
        // Benefits cap at 6.67 and bottom out at 0.
        var scaledStanding = Math.Clamp(stationOwnerStandings, 0d, 6.67d);
        var scalingFactor = baseReprocessingTax / 6.67m;
        var taxReduction = (Tax)scaledStanding * scalingFactor;
        return baseReprocessingTax - taxReduction;
    }

    /// <summary>
    /// Seller always pays the sales "transaction tax", even on immediate orders. This is paid to the Secure Commerce  
    /// Commission (SCC).
    /// </summary>
    /// <remarks>
    /// Calculation derived from <see href="https://wiki.eveuniversity.org/Trading#Sales_tax"/>
    /// </remarks>
    public static Tax SalesTransactionTax(SkillLevel accountingSkill)
    {
        const Tax baseTax = 0.045m;

        accountingSkill = Math.Clamp(accountingSkill, 0, 5);
        return baseTax * (1 - (0.11m * accountingSkill));
    }
}
