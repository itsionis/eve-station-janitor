using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace EveStationJanitor.Core.DataAccess.Entities;

public class Character
{
    public int Id { get; init; }
    public required string Name { get; set; }
    public required int EveCharacterId { get; set; }
    public required string CharacterOwnerHash { get; set; }
    public int? AllianceId { get; set; }
    public required int CorporationId { get; set; }
    public int? FactionId { get; set; }
}

public class CharacterConfiguration : IEntityTypeConfiguration<Character>
{
    public void Configure(EntityTypeBuilder<Character> builder)
    {
        builder.ToTable("Characters");

        builder.HasKey(nameof(Character.Id));

        builder.Property(nameof(Character.Id))
            .ValueGeneratedOnAdd();
        
        builder.Property(nameof(Character.CharacterOwnerHash))
            .HasMaxLength(28);

        builder.Property(nameof(Character.Name))
            .HasMaxLength(128);

        builder.HasIndex(c => c.EveCharacterId)
            .IsUnique();
    }
}
