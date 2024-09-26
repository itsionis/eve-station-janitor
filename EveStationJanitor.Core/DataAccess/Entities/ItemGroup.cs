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
    public required int Id { get; set; }

    public required string Name { get; set; }

    public required int CategoryId { get; set; }

    public required ItemCategory Category { get; set; }

    public IReadOnlyCollection<ItemType> ItemTypes => _itemTypes;
}

public class ItemGroupConfiguration : IEntityTypeConfiguration<ItemGroup>
{
    public void Configure(EntityTypeBuilder<ItemGroup> builder)
    {
        builder.ToTable("ItemGroups");

        builder.HasKey(nameof(ItemGroup.Id));

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
