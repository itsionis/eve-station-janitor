using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace EveStationJanitor.Core.DataAccess.Entities;

[DebuggerDisplay("{Name}")]
public class ItemGroup
{
    private List<ItemType> _itemTypes = [];

    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public required int Id { get; init; }

    public required string Name { get; set; }

    public required int CategoryId { get; set; }

    public ItemCategory Category { get; set; } = null!;

    public IReadOnlyCollection<ItemType> ItemTypes => _itemTypes;
}

public class ItemGroupConfiguration : IEntityTypeConfiguration<ItemGroup>
{
    public void Configure(EntityTypeBuilder<ItemGroup> builder)
    {
        builder.ToTable("ItemGroups");

        builder.HasKey(nameof(ItemGroup.Id));

        builder.Property(nameof(ItemGroup.Id))
            .ValueGeneratedNever();

        // Longest item group as of 2024/11/14 is 73 "Structure Combat Rig L - Point Defense Battery Application and Projection"
        builder.Property(nameof(ItemGroup.Name))
            .HasMaxLength(128);
        
        builder.HasOne(ig => ig.Category)
            .WithMany()
            .HasForeignKey(ig => ig.CategoryId);

        builder.HasMany(ig => ig.ItemTypes)
            .WithOne(it => it.Group)
            .HasForeignKey(it => it.GroupId);

        builder.Metadata
            .FindNavigation(nameof(ItemGroup.ItemTypes))
            ?.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
