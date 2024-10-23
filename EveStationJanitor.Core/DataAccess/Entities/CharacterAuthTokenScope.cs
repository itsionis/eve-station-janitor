using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace EveStationJanitor.Core.DataAccess.Entities;

public class CharacterAuthTokenScope
{
    public int Id { get; set; }
    public required string Scope { get; set; }

    public required int CharacterAuthTokenId { get; set; }
    public CharacterAuthToken CharacterAuthToken { get; set; } = null!;
}

public class AuthTokenScopeConfiguration : IEntityTypeConfiguration<CharacterAuthTokenScope>
{
    public void Configure(EntityTypeBuilder<CharacterAuthTokenScope> builder)
    {
        builder.ToTable("CharacterAuthTokenScopes");

        builder.HasKey(nameof(CharacterAuthTokenScope.Id));

        builder.HasOne(s => s.CharacterAuthToken)
            .WithMany(t => t.Scopes)
            .HasForeignKey(s => s.CharacterAuthTokenId);
    }
}