using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EveStationJanitor.Core.DataAccess.Entities;

public class ItemTypeMaterial
{
    public required int ItemTypeId { get; init; }

    public required int MaterialItemTypeId { get; init; }

    public required int Quantity { get; init; }

    public required ItemType ItemType { get; init; }

    public required ItemType MaterialType { get; init; }
}

public class InventoryTypeMaterialConfiguration : IEntityTypeConfiguration<ItemTypeMaterial>
{
    public void Configure(EntityTypeBuilder<ItemTypeMaterial> builder)
    {
        builder.ToTable("ItemTypeMaterials");

        builder.HasKey(tm => new { tm.ItemTypeId, tm.MaterialItemTypeId });

        builder.Property(nameof(ItemTypeMaterial.ItemTypeId))
            .ValueGeneratedNever();

        builder.Property(nameof(ItemTypeMaterial.MaterialItemTypeId))
            .ValueGeneratedNever();

        builder.HasOne(tm => tm.ItemType)
            .WithMany(it => it.Materials)
            .HasForeignKey(tm => tm.ItemTypeId);

        builder.HasOne(tm => tm.MaterialType)
            .WithMany()
            .HasForeignKey(tm => tm.MaterialItemTypeId);
    }
}