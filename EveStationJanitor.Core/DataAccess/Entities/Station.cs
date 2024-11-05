using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace EveStationJanitor.Core.DataAccess.Entities;

public class Station
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public long Id { get; set; }

    public MapSolarSystem SolarSystem { get; set; } = null!;

    public required int SolarSystemId { get; set; }

    public required int OwnerCorporationId { get; set; }

    public required string Name { get; set; }

    public required double ReprocessingEfficiency { get; set; }

    public required double ReprocessingTax { get; set; }
}

public class StationConfiguration : IEntityTypeConfiguration<Station>
{
    public void Configure(EntityTypeBuilder<Station> builder)
    {
        builder.ToTable("Stations");

        builder.HasOne(station => station.SolarSystem)
            .WithMany(system => system.Stations)
            .HasForeignKey(station => station.SolarSystemId);

        builder.HasIndex(s => s.Id)
            .IsUnique()
            .IsDescending();
    }
}
