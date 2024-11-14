using EveStationJanitor.Core.Eve.Formula;
using Xunit;

namespace EveStationJanitor.Core.Tests.Eve.Formula;

public class StationReprocessingEquipmentTaxFormulaTests
{
    [Theory]
    [InlineData(0, 0.5)]
    [InlineData(3.335, 0.25)]
    [InlineData(6.67, 0)]
    public void EquipmentTax(double standings, decimal expected)
    {
        const decimal fixedBaseReprocessingTax = 0.5m;
        var tax = TaxFormula.StationReprocessingEquipmentTax(fixedBaseReprocessingTax, standings);
        Assert.Equal(expected, tax);
    }
    
    [Theory]
    [InlineData(-1, 0.5)]
    [InlineData(6.7, 0)]
    public void EquipmentTaxIgnoresOutOfRangeStandings(double standings, decimal expected)
    {
        const decimal fixedBaseReprocessingTax = 0.5m;
        var tax = TaxFormula.StationReprocessingEquipmentTax(fixedBaseReprocessingTax, standings);
        Assert.Equal(expected, tax);
    }
}
