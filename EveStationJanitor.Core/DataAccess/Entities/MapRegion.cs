using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace EveStationJanitor.Core.DataAccess.Entities;

[DebuggerDisplay("{Name}")]
public class MapRegion
{
    private List<MapSolarSystem> _systems = [];

    public required int Id { get; init; }

    public required string Name { get; init; }

    public IReadOnlyCollection<MapSolarSystem> Systems => _systems;
}

public class MapRegionConfiguration : IEntityTypeConfiguration<MapRegion>
{
    public void Configure(EntityTypeBuilder<MapRegion> builder)
    {
        builder.ToTable("MapRegions");

        builder.HasKey(nameof(MapRegion.Id));
        
        builder.Property(nameof(MapRegion.Id))
            .ValueGeneratedNever();
        
        builder.HasMany(region => region.Systems)
            .WithOne(system => system.Region)
            .HasForeignKey(system => system.RegionId);

        builder.Metadata
            .FindNavigation(nameof(MapRegion.Systems))
            ?.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
