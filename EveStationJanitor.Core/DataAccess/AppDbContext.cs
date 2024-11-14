using EveStationJanitor.Core.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace EveStationJanitor.Core.DataAccess;

public class AppDbContext : DbContext
{
    public DbSet<ItemTypeMaterial> ItemTypeMaterials { get; set; }
    public DbSet<ItemType> ItemTypes { get; set; }
    public DbSet<ItemGroup> ItemGroups { get; set; }
    public DbSet<ItemCategory> ItemCategories { get; set; }
    public DbSet<MapRegion> MapRegions { get; set; }
    public DbSet<MapSolarSystem> MapSolarSystems { get; set; }
    public DbSet<EntityTag> EntityTags { get; set; }
    public DbSet<MarketOrder> MarketOrders { get; set; }
    public DbSet<Station> Stations { get; set; }
    public DbSet<Character> Characters { get; set; }
    public DbSet<CharacterAuthToken> CharacterAuthTokens { get; set; }
    public DbSet<CharacterAuthTokenScope> CharacterAuthTokenScopes { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CharacterConfiguration());
        modelBuilder.ApplyConfiguration(new CharacterAuthTokenConfiguration());
        modelBuilder.ApplyConfiguration(new CharacterAuthTokenScopeConfiguration());
        
        modelBuilder.ApplyConfiguration(new EntityTagConfiguration());
        
        modelBuilder.ApplyConfiguration(new ItemCategoryConfiguration());
        modelBuilder.ApplyConfiguration(new ItemGroupConfiguration());
        modelBuilder.ApplyConfiguration(new ItemTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ItemTypeMaterialConfiguration());
        
        modelBuilder.ApplyConfiguration(new MapRegionConfiguration());
        modelBuilder.ApplyConfiguration(new MapSolarSystemConfiguration());
        modelBuilder.ApplyConfiguration(new StationConfiguration());
        
        modelBuilder.ApplyConfiguration(new MarketOrderConfiguration());
    }
}
