using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace EveStationJanitor.Core.DataAccess.Entities;

[DebuggerDisplay("{Name}")]
public class ItemType
{
    private List<ItemTypeMaterial> _materials = [];

    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required int GroupId { get; set; }
    public required ItemGroup Group { get; set; }
    public required float Volume { get; set; }
    public required float Mass { get; set; }
    public int PortionSize { get; set; }

    public IReadOnlyList<ItemTypeMaterial> Materials => _materials;

}

public class ItemTypeConfiguration : IEntityTypeConfiguration<ItemType>
{
    public void Configure(EntityTypeBuilder<ItemType> builder)
    {
        builder.ToTable("ItemTypes");

        builder.HasKey(nameof(ItemType.Id));

        builder.HasOne(it => it.Group)
            .WithMany(ig => ig.ItemTypes)
            .HasForeignKey(it => it.GroupId);
        
        builder.HasMany(it => it.Materials)
            .WithOne(it => it.ItemType)
            .HasForeignKey(it=>it.ItemTypeId);

        builder.Metadata
            .FindNavigation(nameof(ItemType.Materials))
            ?.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
