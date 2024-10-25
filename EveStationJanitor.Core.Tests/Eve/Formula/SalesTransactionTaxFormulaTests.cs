using EveStationJanitor.Core.Eve.Formula;
using Xunit;

namespace EveStationJanitor.Core.Tests.Eve.Formula;

public class SalesTransactionTaxFormulaTests
{
    [Theory]
    [InlineData(0, 0.04500)]
    [InlineData(1, 0.04005)]
    [InlineData(2, 0.03510)]
    [InlineData(3, 0.03015)]
    [InlineData(4, 0.02520)]
    [InlineData(5, 0.02025)]
    public void TaxRateIsBasedOnAccountingSkill(int accounting, decimal expected)
    {
        var tax = TaxFormula.SalesTransactionTax(accounting);
        Assert.Equal(expected, tax);
    }

    [Theory]
    [InlineData(-1, 0.04500)]
    [InlineData(6, 0.02025)]
    public void TaxRateIgnoresOutOfRangeSkillLevels(int accounting, decimal expected)
    {
        var tax = TaxFormula.SalesTransactionTax(accounting);
        Assert.Equal(expected, tax);
    }
}
