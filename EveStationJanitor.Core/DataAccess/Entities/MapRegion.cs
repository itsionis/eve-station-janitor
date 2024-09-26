using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace EveStationJanitor.Core.DataAccess.Entities;

[DebuggerDisplay("{Name}")]
public class MapRegion
{
    private List<MapSolarSystem> _systems = [];

    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public required int Id { get; set; }

    public required string Name { get; set; }

    public IReadOnlyList<MapSolarSystem> Systems => _systems;
}

public class MapRegionConfiguration : IEntityTypeConfiguration<MapRegion>
{
    public void Configure(EntityTypeBuilder<MapRegion> builder)
    {
        builder.ToTable("MapRegions");

        builder.HasKey(nameof(MapRegion.Id));

        builder.HasMany(region => region.Systems)
            .WithOne(system => system.Region)
            .HasForeignKey(system => system.RegionId);

        builder.Metadata
            .FindNavigation(nameof(MapRegion.Systems))
            ?.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
