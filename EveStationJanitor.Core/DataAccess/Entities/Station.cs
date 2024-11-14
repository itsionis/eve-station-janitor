using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EveStationJanitor.Core.DataAccess.Entities;

public class Station
{
    public long Id { get; init; }

    public MapSolarSystem SolarSystem { get; init; } = null!;

    public required int SolarSystemId { get; init; }

    public required CorporationId OwnerCorporationId { get; init; }

    public required string Name { get; init; }

    public required YieldPercent ReprocessingEfficiency { get; init; }

    public required Tax ReprocessingTax { get; init; }
}

public class StationConfiguration : IEntityTypeConfiguration<Station>
{
    public void Configure(EntityTypeBuilder<Station> builder)
    {
        builder.ToTable("Stations");

        builder.HasKey(nameof(Station.Id));

        builder.Property(nameof(Station.Id))
            .ValueGeneratedNever();

        // Longest station name as of 2024/11/24 is 78 characters "Kor-Azor Prime IV (Eclipticum) - Moon Griklaeum - Ishukone Corporation Factory"
        builder.Property(nameof(Station.Name))
            .HasMaxLength(256);
        
        builder.HasOne(station => station.SolarSystem)
            .WithMany(system => system.Stations)
            .HasForeignKey(station => station.SolarSystemId);

        builder.HasIndex(s => s.Id)
            .IsUnique()
            .IsDescending();
    }
}
