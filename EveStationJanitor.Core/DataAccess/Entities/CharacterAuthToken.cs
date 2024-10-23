using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using System.ComponentModel.DataAnnotations.Schema;

namespace EveStationJanitor.Core.DataAccess.Entities;

public class CharacterAuthToken
{
    private readonly List<CharacterAuthTokenScope> _scopes = [];

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public required byte[] EncryptedRefreshToken { get; set; }
    public required Instant ExpiresOn { get; set; }
    
    public required int CharacterId { get; set; }
    public Character Character { get; set; } = null!;

    public IReadOnlyCollection<CharacterAuthTokenScope> Scopes => _scopes;

    public void ClearScopes()
    {
        _scopes.Clear();
    }

    public void AddScope(string scope)
    {
        var tokenScope = new CharacterAuthTokenScope { Scope = scope, CharacterAuthTokenId = Id };
        _scopes.Add(tokenScope);
    }
}

public class CharacterAuthTokenConfiguration : IEntityTypeConfiguration<CharacterAuthToken>
{
    public void Configure(EntityTypeBuilder<CharacterAuthToken> builder)
    {
        builder.ToTable("CharacterAuthTokens");

        builder.HasKey(nameof(CharacterAuthToken.Id));

        builder.HasOne(t => t.Character)
            .WithMany()
            .HasForeignKey(t => t.CharacterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.Scopes)
            .WithOne(s => s.CharacterAuthToken)
            .HasForeignKey(s => s.CharacterAuthTokenId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata
            .FindNavigation(nameof(CharacterAuthToken.Scopes))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
