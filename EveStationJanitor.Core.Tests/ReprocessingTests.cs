using EveStationJanitor.Core.Eve;
using Xunit;

namespace EveStationJanitor.Core.Tests;

public class ReprocessingTests(ReprocessingTestFixture fixture) : IClassFixture<ReprocessingTestFixture>
{
    [Fact]
    public void ZeroBonuses_BaseYield()
    {
        AssertReprocessing(_ => { },
            tritanium: 87,
            mexallon: 35);
    }

    [Fact]
    public void PerfectOreReprocessing_ImprovedYield()
    {
        AssertReprocessing(skills => { skills.SimpleOreProcessing = 5; },
            tritanium: 96,
            mexallon: 38);
    }

    [Fact]
    public void PerfectReprocessing_ImprovedYield()
    {
        AssertReprocessing(skills => { skills.Reprocessing = 5; },
            tritanium: 100,
            mexallon: 40);
    }

    [Fact]
    public void PerfectReprocessingEfficiency_ImprovedYield()
    {
        AssertReprocessing(skills => { skills.ReprocessingEfficiency = 5; },
            tritanium: 96,
            mexallon: 38);
    }
    
    private void AssertReprocessing(Action<Skills> configureSkills, int tritanium, int mexallon)
    {
        var skills = new Skills();
        configureSkills(skills);

        var standings = new Standings(skills);
        var implants = new CloneImplants();

        var reprocessing = new StationReprocessing(new OreReprocessing(skills), fixture.JitaStation, skills, standings,
            implants);

        Assert.Equal(tritanium,
            reprocessing.ReprocessedMaterialQuantity(fixture.PlagioclaseMaterials[fixture.Tritanium.Id]));
        
        Assert.Equal(mexallon,
            reprocessing.ReprocessedMaterialQuantity(fixture.PlagioclaseMaterials[fixture.Mexallon.Id]));
    }
}
