using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace EveStationJanitor.Core.DataAccess.Entities;

[DebuggerDisplay("{Name}")]
public class MapSolarSystem
{
    private readonly List<Station> _stations = [];

    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public required int Id { get; set; }

    public required string Name { get; set; }

    public required int RegionId { get; set; }

    public required MapRegion Region { get; set; }

    public List<Station> Stations => _stations;
}

public class MapSolarSystemConfiguration : IEntityTypeConfiguration<MapSolarSystem>
{
    public void Configure(EntityTypeBuilder<MapSolarSystem> builder)
    {
        builder.ToTable("MapSolarSystems");

        builder.HasKey(nameof(MapSolarSystem.Id));

        builder.HasMany(system => system.Stations)
            .WithOne(station => station.SolarSystem)
            .HasForeignKey(station => station.SolarSystemId);

        builder.Metadata
            .FindNavigation(nameof(MapSolarSystem.Stations))
            ?.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
