﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace EveStationJanitor.Core.DataAccess.Entities;

[DebuggerDisplay("{Name}")]
public class MapSolarSystem
{
    private readonly List<Station> _stations = [];

    public required int Id { get; init; }

    public required string Name { get; init; }

    public required int RegionId { get; init; }

    public required MapRegion Region { get; init; }

    public IReadOnlyCollection<Station> Stations => _stations;
}

public class MapSolarSystemConfiguration : IEntityTypeConfiguration<MapSolarSystem>
{
    public void Configure(EntityTypeBuilder<MapSolarSystem> builder)
    {
        builder.ToTable("MapSolarSystems");

        builder.HasKey(nameof(MapSolarSystem.Id));

        builder.Property(nameof(MapSolarSystem.Id))
            .ValueGeneratedNever();

        // Longest system name as of 2024/11/14 is 17 characters "Tash-Murkon Prime"
        builder.Property(nameof(MapSolarSystem.Name))
            .HasMaxLength(32);

        builder.HasMany(system => system.Stations)
            .WithOne(station => station.SolarSystem)
            .HasForeignKey(station => station.SolarSystemId);

        builder.Metadata
            .FindNavigation(nameof(MapSolarSystem.Stations))
            ?.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
