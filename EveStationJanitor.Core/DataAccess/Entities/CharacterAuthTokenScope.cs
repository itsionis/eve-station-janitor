using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace EveStationJanitor.Core.DataAccess.Entities;

public class CharacterAuthTokenScope
{
    public int Id { get; init; }
    public required string Scope { get; init; }
    public required int CharacterAuthTokenId { get; init; }
    public CharacterAuthToken CharacterAuthToken { get; init; } = null!;
}

public class CharacterAuthTokenScopeConfiguration : IEntityTypeConfiguration<CharacterAuthTokenScope>
{
    public void Configure(EntityTypeBuilder<CharacterAuthTokenScope> builder)
    {
        builder.ToTable("CharacterAuthTokenScopes");

        builder.HasKey(nameof(CharacterAuthTokenScope.Id));

        builder.Property(nameof(CharacterAuthTokenScope.Id))
            .ValueGeneratedOnAdd();
        
        builder.Property(nameof(CharacterAuthTokenScope.Scope))
            .HasMaxLength(128);
        
        builder.HasOne(s => s.CharacterAuthToken)
            .WithMany(t => t.Scopes)
            .HasForeignKey(s => s.CharacterAuthTokenId);
    }
}
