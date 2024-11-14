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

        builder.Property(nameof(ItemCategory.Id))
            .ValueGeneratedNever();
    }
}
