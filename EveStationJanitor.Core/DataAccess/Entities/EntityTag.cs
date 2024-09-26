using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EveStationJanitor.Core.DataAccess.Entities;

public class EntityTag
{
    public required string Key { get; set; }

    public required string Tag { get; set; }
}

public class StaticDataETagConfiguration : IEntityTypeConfiguration<EntityTag>
{
    public void Configure(EntityTypeBuilder<EntityTag> builder)
    {
        builder.ToTable("EntityTags");

        builder.HasKey(nameof(EntityTag.Key));
    }
}
