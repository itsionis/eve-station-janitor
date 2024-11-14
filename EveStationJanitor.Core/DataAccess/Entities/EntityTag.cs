using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EveStationJanitor.Core.DataAccess.Entities;

public class EntityTag
{
    public required string Key { get; init; }

    public required string Tag { get; set; }
}

public class EntityTagConfiguration : IEntityTypeConfiguration<EntityTag>
{
    public void Configure(EntityTypeBuilder<EntityTag> builder)
    {
        builder.ToTable("EntityTags");
        
        builder.HasKey(nameof(EntityTag.Key));

        builder.Property(nameof(EntityTag.Key))
            .ValueGeneratedNever();
    }
}
