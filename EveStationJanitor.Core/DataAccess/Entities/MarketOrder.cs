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
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public required long OrderId { get; set; }

    public required Duration Duration { get; set; }

    public required bool IsBuyOrder { get; set; }

    public required Instant Issued { get; set; }

    [NotMapped]
    public Instant Expires => Issued.Plus(Duration);

    public Station Station { get; set; } = null!;

    public required long LocationId { get; set; }

    public required int MinVolume { get; set; }

    public required double Price { get; set; }

    public required OrderRange Range { get; set; }

    public required int SystemId { get; set; }

    public MapSolarSystem SolarSystem { get; set; } = null!;

    public required int TypeId { get; set; }

    public ItemType ItemType { get; set; } = null!;

    public required long VolumeRemaining { get; set; }

    public required long VolumeTotal { get; set; }

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
