using CsvHelper;
using CsvHelper.Configuration;
using EveStationJanitor.Core.DataAccess;
using EveStationJanitor.Core.DataAccess.Entities;
using EveStationJanitor.EveApi;
using ICSharpCode.SharpZipLib.BZip2;
using System.Globalization;
using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;

namespace EveStationJanitor.Core.StaticData;

public class FuzzworksStaticDataDownloader
{
    private readonly AppDbContext _context;
    private readonly IEntityTagProvider _entityTagProvider;
    private readonly IHttpClientFactory _clientFactory;

    public FuzzworksStaticDataDownloader(AppDbContext context, IEntityTagProvider entityTagProvider,
        IHttpClientFactory clientFactory)
    {
        _context = context;
        _entityTagProvider = entityTagProvider;
        _clientFactory = clientFactory;
    }

    private async Task GetStaticData<TEntity, TMapper>(
        HttpClient client,
        string fileName,
        Func<DbSet<TEntity>> getEntitySet,
        Func<DbSet<TEntity>, TEntity, Task<bool>> entityExists)
        where TEntity : class
        where TMapper : ClassMap<TEntity>
    {
        var entityTag = _entityTagProvider.GetEntityTag(fileName);
        var request = new HttpRequestMessage(HttpMethod.Get, fileName);

        if (entityTag is not null)
        {
            var entityDataTagValue = new EntityTagHeaderValue(entityTag);
            request.Headers.IfNoneMatch.Add(entityDataTagValue);
        }

        var response = await client.SendAsync(request);
        if (response.StatusCode == System.Net.HttpStatusCode.NotModified)
        {
            return;
        }

        response.EnsureSuccessStatusCode();

        var dataFileName = Path.GetFileNameWithoutExtension(fileName);
        if (string.IsNullOrWhiteSpace(dataFileName))
        {
            throw new InvalidOperationException("Cannot resolve file name of static data dump");
        }

        var responseEntityTag = response.Headers.ETag?.Tag;
        if (responseEntityTag is not null)
        {
            _entityTagProvider.SetEntityTag(fileName, responseEntityTag);
        }

        var inStream = await response.Content.ReadAsStreamAsync();
        var outStream = File.OpenWrite(dataFileName);
        BZip2.Decompress(inStream, outStream, true);

        using var reader = File.OpenText(dataFileName);
        using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);

        csvReader.Context.RegisterClassMap<TMapper>();
        var records = csvReader.GetRecords<TEntity>().ToList();

        var set = getEntitySet();

        foreach (var record in records)
        {
            if (await entityExists(set, record))
            {
                set.Update(record);
            }
            else
            {
                set.Add(record);
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task Run()
    {
        var client = _clientFactory.CreateClient("static-data");

        // Inventory
        await GetStaticData<ItemCategory, InventoryCategoryCsvMap>(client, "dump/latest/invCategories.csv.bz2",
            () => _context.ItemCategories,
            (set, itemCategory) => set.AnyAsync(e => e.Id == itemCategory.Id));

        await GetStaticData<ItemGroup, InventoryGroupCsvMap>(client, "dump/latest/invGroups.csv.bz2",
            () => _context.ItemGroups,
            (set, itemGroup) => set.AnyAsync(e => e.Id == itemGroup.Id));

        await GetStaticData<ItemType, ItemTypeTypeCsvMap>(client, "dump/latest/invTypes.csv.bz2",
            () => _context.ItemTypes,
            (set, itemType) => set.AnyAsync(e => e.Id == itemType.Id));

        await GetStaticData<ItemTypeMaterial, InventoryTypeMaterialCsvMap>(client,
            "dump/latest/invTypeMaterials.csv.bz2",
            () => _context.ItemTypeMaterials,
            (set, itemTypeMaterial) => set.AnyAsync(e => e.ItemTypeId == itemTypeMaterial.ItemTypeId && e.MaterialItemTypeId == itemTypeMaterial.MaterialItemTypeId));

        // Map
        await GetStaticData<MapRegion, MapRegionsCsvMap>(client, "dump/latest/mapRegions.csv.bz2",
            () => _context.MapRegions,
            (set, region) => set.AnyAsync(e => e.Id == region.Id));

        await GetStaticData<MapSolarSystem, MapSolarSystemsCsvMap>(client, "dump/latest/mapSolarSystems.csv.bz2",
            () => _context.MapSolarSystems,
            (set, system) => set.AnyAsync(e => e.Id == system.Id));

        await GetStaticData<Station, StationCsvMap>(client, "dump/latest/staStations.csv.bz2",
            () => _context.Stations,
            (set, station) => set.AnyAsync(e => e.Id == station.Id));
    }
}

public sealed class MapRegionsCsvMap : ClassMap<MapRegion>
{
    public MapRegionsCsvMap()
    {
        Map(r => r.Id).Name("regionID");
        Map(r => r.Name).Name("regionName");
    }
}

public sealed class MapSolarSystemsCsvMap : ClassMap<MapSolarSystem>
{
    public MapSolarSystemsCsvMap()
    {
        Map(r => r.Id).Name("solarSystemID");
        Map(r => r.Name).Name("solarSystemName");
        Map(r => r.RegionId).Name("regionID");
    }
}

public sealed class InventoryTypeMaterialCsvMap : ClassMap<ItemTypeMaterial>
{
    public InventoryTypeMaterialCsvMap()
    {
        Map(m => m.ItemTypeId).Name("typeID");
        Map(m => m.MaterialItemTypeId).Name("materialTypeID");
        Map(m => m.Quantity).Name("quantity");
    }
}

public sealed class ItemTypeTypeCsvMap : ClassMap<ItemType>
{
    public ItemTypeTypeCsvMap()
    {
        Map(m => m.Id).Name("typeID");
        Map(m => m.GroupId).Name("groupID");
        Map(m => m.Name).Name("typeName");
        Map(m => m.Volume).Name("volume");
        Map(m => m.Mass).Name("mass");
        Map(m => m.PortionSize).Name("portionSize");
    }
}

public sealed class InventoryGroupCsvMap : ClassMap<ItemGroup>
{
    public InventoryGroupCsvMap()
    {
        Map(m => m.Id).Name("groupID");
        Map(m => m.CategoryId).Name("categoryID");
        Map(m => m.Name).Name("groupName");
    }
}

public sealed class InventoryCategoryCsvMap : ClassMap<ItemCategory>
{
    public InventoryCategoryCsvMap()
    {
        Map(m => m.Id).Name("categoryID");
        Map(m => m.Name).Name("categoryName");
    }
}

public sealed class StationCsvMap : ClassMap<Station>
{
    public StationCsvMap()
    {
        Map(s => s.Id).Name("stationID");
        Map(s => s.SolarSystemId).Name("solarSystemID");
        Map(s => s.OwnerCorporationId).Name("corporationID");
        Map(s => s.Name).Name("stationName");
        Map(s => s.ReprocessingEfficiency).Name("reprocessingEfficiency");
        Map(s => s.ReprocessingTax).Name("reprocessingStationsTake");
    }
}