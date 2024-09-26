using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace EveStationJanitor.Core.DataAccess.Entities;

public class ItemTypeMaterial
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public required int ItemTypeId { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public required int MaterialItemTypeId { get; set; }

    public required int Quantity { get; set; }

    public required ItemType ItemType { get; set; }

    public required ItemType MaterialType { get; set; }
}

public class InventoryTypeMaterialConfiguration : IEntityTypeConfiguration<ItemTypeMaterial>
{
    public void Configure(EntityTypeBuilder<ItemTypeMaterial> builder)
    {
        builder.ToTable("ItemTypeMaterials");

        builder.HasKey(tm => new { tm.ItemTypeId, tm.MaterialItemTypeId });

        builder.HasOne(tm => tm.ItemType)
            .WithMany(it => it.Materials)
            .HasForeignKey(tm => tm.ItemTypeId);

        builder.HasOne(tm => tm.MaterialType)
            .WithMany()
            .HasForeignKey(tm => tm.MaterialItemTypeId);
    }
}
