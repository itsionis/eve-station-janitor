namespace EveStationJanitor.Core.Eve.Formula;

public static class TaxFormula
{
    public static decimal StationReprocessingEquipmentTax(double baseReprocessingTax, double stationOwnerStandings)
    {
        // Calculation derived from https://wiki.eveuniversity.org/Tax#Equipment_tax

        // Benefits cap at 6.67 and bottom out at 0.
        var scaledStanding = Math.Clamp(stationOwnerStandings, 0d, 6.67d);
        var scalingFactor = baseReprocessingTax / 6.67d;
        var taxReduction = (decimal)(scaledStanding * scalingFactor);
        return (decimal)baseReprocessingTax - taxReduction;
    }
}
