using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics;

namespace EveStationJanitor.Core.DataAccess.Entities;

[DebuggerDisplay("{Name}")]
public class ItemCategory
{
    public required int Id { get; init; }

    public required string Name { get; init; }
}

public class ItemCategoryConfiguration : IEntityTypeConfiguration<ItemCategory>
{
    public void Configure(EntityTypeBuilder<ItemCategory> builder)
    {
        builder.ToTable("ItemCategories");
        
        builder.HasKey(nameof(ItemCategory.Id));

        // Longest category as of 2024/11/14 is 23 characters "Infrastructure Upgrades"
        builder.Property(nameof(ItemCategory.Name)).HasMaxLength(64);
        
        builder.Property(nameof(ItemCategory.Id))
            .ValueGeneratedNever();
    }
}
