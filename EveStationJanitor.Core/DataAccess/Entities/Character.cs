using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace EveStationJanitor.Core.DataAccess.Entities;

public class Character
{
    /// <summary>Internal ID to this application</summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; private set; }
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

        builder.HasIndex(c => c.EveCharacterId)
            .IsUnique();
    }
}
