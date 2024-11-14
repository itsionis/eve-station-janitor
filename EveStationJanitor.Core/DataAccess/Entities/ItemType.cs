using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics;

namespace EveStationJanitor.Core.DataAccess.Entities;

[DebuggerDisplay("{Name}")]
public class ItemType
{
    private List<ItemTypeMaterial> _materials = [];

    public required int Id { get; init; }
    public required string Name { get; init; }
    public required int GroupId { get; init; }
    public ItemGroup Group { get; init; } = null!;
    public required float Volume { get; init; }
    public required float Mass { get; init; }
    public int PortionSize { get; init; }

    public IReadOnlyCollection<ItemTypeMaterial> Materials => _materials;

    public ItemTypeMaterial AddMaterial(ItemType material, int quantity)
    {
        var itemTypeMaterial = new ItemTypeMaterial
        {
            ItemType = this,
            ItemTypeId = Id,
            MaterialType = material,
            MaterialItemTypeId = material.Id,
            Quantity = quantity 
        };

        _materials.Add(itemTypeMaterial);
        return itemTypeMaterial;
    }
}

public class ItemTypeConfiguration : IEntityTypeConfiguration<ItemType>
{
    public void Configure(EntityTypeBuilder<ItemType> builder)
    {
        builder.ToTable("ItemTypes");

        builder.HasKey(nameof(ItemType.Id));

        builder.Property(nameof(ItemType.Id))
            .ValueGeneratedNever();

        builder.HasOne(it => it.Group)
            .WithMany(ig => ig.ItemTypes)
            .HasForeignKey(it => it.GroupId);

        builder.HasMany(it => it.Materials)
            .WithOne(it => it.ItemType)
            .HasForeignKey(it => it.ItemTypeId);

        builder.Metadata
            .FindNavigation(nameof(ItemType.Materials))
            ?.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
