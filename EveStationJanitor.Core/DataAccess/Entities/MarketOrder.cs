using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NodaTime;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace EveStationJanitor.Core.DataAccess.Entities;

[DebuggerDisplay("{VolumeRemaining} {ItemType?.Name} @ {Price}")]
public class MarketOrder
{
    public required long OrderId { get; init; }

    public required Duration Duration { get; init; }

    public required bool IsBuyOrder { get; init; }

    public required Instant Issued { get; init; }

    [NotMapped] public Instant Expires => Issued.Plus(Duration);

    public Station Station { get; init; } = null!;

    public required long LocationId { get; init; }

    public required int MinVolume { get; init; }

    public required double Price { get; init; }

    public required OrderRange Range { get; init; }

    public required int SystemId { get; init; }

    public MapSolarSystem SolarSystem { get; init; } = null!;

    public required int TypeId { get; init; }

    public ItemType ItemType { get; init; } = null!;

    public required long VolumeRemaining { get; init; }

    public required long VolumeTotal { get; init; }

    public static OrderRange ParseOrderRange(string range)
    {
        return range switch
        {
            "station" => OrderRange.Station,
            "region" => OrderRange.Region,
            "solarsystem" => OrderRange.SolarSystem,
            "1" => OrderRange.Jumps1,
            "2" => OrderRange.Jumps2,
            "3" => OrderRange.Jumps3,
            "4" => OrderRange.Jumps4,
            "5" => OrderRange.Jumps5,
            "10" => OrderRange.Jumps10,
            "20" => OrderRange.Jumps20,
            "30" => OrderRange.Jumps30,
            "40" => OrderRange.Jumps40,
            _ => OrderRange.Station,
        };
    }
}

public enum OrderRange
{
    Station,
    Region,
    SolarSystem,
    Jumps1,
    Jumps2,
    Jumps3,
    Jumps4,
    Jumps5,
    Jumps10,
    Jumps20,
    Jumps30,
    Jumps40,
}

public class MarketOrderConfiguration : IEntityTypeConfiguration<MarketOrder>
{
    public void Configure(EntityTypeBuilder<MarketOrder> builder)
    {
        builder.ToTable("MarketOrders");

        builder.HasKey(nameof(MarketOrder.OrderId));

        builder.Property(nameof(MarketOrder.OrderId))
            .ValueGeneratedNever();

        builder.HasOne(order => order.ItemType)
            .WithMany()
            .HasForeignKey(order => order.TypeId);

        builder.HasOne(order => order.SolarSystem)
            .WithMany()
            .HasForeignKey(order => order.SystemId);

        builder.HasOne(order => order.Station)
            .WithMany()
            .HasForeignKey(station => station.LocationId);

        var converter = new ValueConverter<Duration, int>(
            convertToProviderExpression: x => (int)x.TotalDays,
            convertFromProviderExpression: x => Duration.FromDays(x));

        builder.Property(order => order.Duration).HasConversion(converter);
    }
}
