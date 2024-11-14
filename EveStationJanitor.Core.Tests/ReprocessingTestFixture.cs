using EveStationJanitor.Core.DataAccess.Entities;

namespace EveStationJanitor.Core.Tests;

public class ReprocessingTestFixture
{
    public ReprocessingTestFixture()
    {
        // Categories
        var categoryMaterial = new ItemCategory { Id = 4, Name = "Material" };
        var categoryAsteroid = new ItemCategory { Id = 25, Name = "Asteroid" };
        var categoryImplant = new ItemCategory { Id = 20, Name = "Implant" };

        // Groups
        var groupMineral = new ItemGroup { Id = 18, Name = "Mineral", CategoryId = 4, Category = categoryMaterial };
        var groupPlagioclase = new ItemGroup { Id = 458, Name = "Plagioclase", CategoryId = categoryAsteroid.Id, Category = categoryAsteroid };
        var groupCyberResourceProcessing = new ItemGroup { Id = 1229, Name = "Cyber Resource Processing", CategoryId = categoryImplant.Id, Category = categoryImplant };
        
        // Station
        JitaStation = new Station
        {
            Id = 60003760,
            Name = "Jita IV - Moon 4 - Caldari Navy Assembly Plant",
            OwnerCorporationId = 1000035,
            ReprocessingEfficiency = 0.5m,
            ReprocessingTax = 0.05m,
            SolarSystemId = 30000142
        };
        
        // Implants
        ZainouBeancounterReprocessingRx801 = new ItemType { Id = 27175, Name = "Zainou 'Beancounter' Reprocessing RX-801", PortionSize = 1, GroupId = groupCyberResourceProcessing.Id, Group = groupCyberResourceProcessing };
        ZainouBeancounterReprocessingRx802 = new ItemType { Id = 27169, Name = "Zainou 'Beancounter' Reprocessing RX-802", PortionSize = 1, GroupId = groupCyberResourceProcessing.Id, Group = groupCyberResourceProcessing };
        ZainouBeancounterReprocessingRx804 = new ItemType { Id = 27174, Name = "Zainou 'Beancounter' Reprocessing RX-804",  PortionSize = 1, GroupId = groupCyberResourceProcessing.Id, Group = groupCyberResourceProcessing };

        // Minerals
        Tritanium = new ItemType { Id = 34, GroupId = 18, Group = groupMineral, Name = "Tritanium" };
        Mexallon = new ItemType { Id = 36, GroupId = 18, Group = groupMineral, Name = "Mexallon" };
        Isogen = new ItemType { Id = 37, GroupId = 18, Group = groupMineral, Name = "Isogen" };
        Nocxium = new ItemType { Id = 38, GroupId = 18, Group = groupMineral, Name = "Nocxium" };
        Megacyte = new ItemType { Id = 40, GroupId = 18, Group = groupMineral, Name = "Megacyte" };

        // Asteroids
        Plagioclase = new ItemType { Id = 18, GroupId = groupPlagioclase.Id, Group = groupPlagioclase, Name = "Plagioclase" };
        AzurePlagioclase = new ItemType { Id = 17455, GroupId = groupPlagioclase.Id, Group = groupPlagioclase, Name = "Azure Plagioclase"};
        
        // Materials from items
        PlagioclaseMaterials = new Dictionary<int, ItemTypeMaterial>
        {
            { Tritanium.Id, Plagioclase.AddMaterial(Tritanium, 175) },
            { Mexallon.Id, Plagioclase.AddMaterial(Mexallon, 70) }
        };
        
        AzurePlagioclaseMaterials = new Dictionary<int, ItemTypeMaterial>
        {
            { Tritanium.Id, AzurePlagioclase.AddMaterial(Tritanium, 184) },
            { Mexallon.Id, AzurePlagioclase.AddMaterial(Mexallon, 74) }
        };
    }
    
    public Station JitaStation { get; }
    public ItemType Tritanium { get; }
    public ItemType Mexallon { get; }
    public ItemType Isogen { get; }
    public ItemType Nocxium { get; }
    public ItemType Megacyte { get; }
    public ItemType Plagioclase { get; }
    public ItemType AzurePlagioclase { get; }
    
    public ItemType ZainouBeancounterReprocessingRx801 { get; }
    public ItemType ZainouBeancounterReprocessingRx802 { get; }
    public ItemType ZainouBeancounterReprocessingRx804 { get; }
    
    public Dictionary<int, ItemTypeMaterial> PlagioclaseMaterials { get; }
    public Dictionary<int, ItemTypeMaterial> AzurePlagioclaseMaterials { get; }
}