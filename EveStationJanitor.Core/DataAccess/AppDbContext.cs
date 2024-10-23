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
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
