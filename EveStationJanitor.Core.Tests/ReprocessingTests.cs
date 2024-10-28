using EveStationJanitor.Core.Eve;
using Xunit;

namespace EveStationJanitor.Core.Tests;

public class ReprocessingTests(ReprocessingTestFixture fixture) : IClassFixture<ReprocessingTestFixture>
{
    [Fact]
    public void ZeroBonuses_BaseYield()
    {
        AssertReprocessing(
            expectedTritanium: 87,
            expectedMexallon: 35);
    }

    [Fact]
    public void PerfectOreReprocessing_ImprovedYield()
    {
        AssertReprocessing(
            skills => skills.SimpleOreProcessing = 5,
            expectedTritanium: 96,
            expectedMexallon: 38);
    }

    [Fact]
    public void PerfectReprocessing_ImprovedYield()
    {
        AssertReprocessing(
            skills => skills.Reprocessing = 5,
            expectedTritanium: 100,
            expectedMexallon: 40);
    }

    [Fact]
    public void PerfectReprocessingEfficiency_ImprovedYield()
    {
        AssertReprocessing(
            skills => skills.ReprocessingEfficiency = 5,
            expectedTritanium: 96,
            expectedMexallon: 38);
    }

    [Fact]
    public void ImplantReprocessingBonus_ImprovedYield()
    {
        AssertReprocessing(
            implants => implants.AddImplant(fixture.ZainouBeancounterReprocessingRx801),
            expectedTritanium: 88,
            expectedMexallon: 35);

        AssertReprocessing(
            implants => implants.AddImplant(fixture.ZainouBeancounterReprocessingRx802),
            expectedTritanium: 89,
            expectedMexallon: 35);

        AssertReprocessing(
            implants => implants.AddImplant(fixture.ZainouBeancounterReprocessingRx804),
            expectedTritanium: 91,
            expectedMexallon: 36);
    }

    private void AssertReprocessing(Action<Skills> configureSkills, int expectedTritanium, int expectedMexallon)
    {
        var skills = new Skills();
        configureSkills(skills);

        var standings = new Standings(skills);
        var implants = new CloneImplants();

        AssertReprocessing(skills, standings, implants, expectedTritanium, expectedMexallon);
    }

    private void AssertReprocessing(int expectedTritanium, int expectedMexallon)
    {
        var skills = new Skills();
        var standings = new Standings(skills);
        var implants = new CloneImplants();

        AssertReprocessing(skills, standings, implants, expectedTritanium, expectedMexallon);
    }

    private void AssertReprocessing(Action<CloneImplants> configureImplants, int expectedTritanium,
        int expectedMexallon)
    {
        var skills = new Skills();
        var standings = new Standings(skills);

        var implants = new CloneImplants();
        configureImplants(implants);

        AssertReprocessing(skills, standings, implants, expectedTritanium, expectedMexallon);
    }

    private void AssertReprocessing(Skills skills, Standings standings, CloneImplants implants, int expectedTritanium,
        int expectedMexallon)
    {
        var reprocessing = new StationReprocessing(new OreReprocessing(skills), fixture.JitaStation, skills, standings,
            implants);

        Assert.Equal(expectedTritanium,
            reprocessing.ReprocessedMaterialQuantity(fixture.PlagioclaseMaterials[fixture.Tritanium.Id]));

        Assert.Equal(expectedMexallon,
            reprocessing.ReprocessedMaterialQuantity(fixture.PlagioclaseMaterials[fixture.Mexallon.Id]));
    }
}
