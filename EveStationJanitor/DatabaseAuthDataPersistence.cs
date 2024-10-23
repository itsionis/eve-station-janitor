using EveStationJanitor.Authentication;
using EveStationJanitor.Authentication.Persistence;
using EveStationJanitor.Core.DataAccess.Entities;
using EveStationJanitor.Core.DataAccess;
using NodaTime;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace EveStationJanitor;

public class DatabaseAuthDataPersistence(AppDbContext context) : IAuthenticationDataProvider
{
    public async Task<AuthorizedToken?> GetAuthTokens(int eveCharacterId)
    {
        var authToken = await context.CharacterAuthTokens
            .Include(t => t.Scopes)
            .Include(t => t.Character)
            .Where(t => t.Character.EveCharacterId == eveCharacterId)
            .FirstOrDefaultAsync();

        if (authToken == null)
            return null;

        var decryptedRefreshToken = DecryptRefreshToken(authToken.EncryptedRefreshToken);

        return new AuthorizedToken
        {
            // Access token isn't stored, will be refreshed
            AccessToken = "",
            RefreshToken = decryptedRefreshToken,
            ExpiresOn = authToken.ExpiresOn,
            Scopes = authToken.Scopes.Select(s => s.Scope).ToHashSet()
        };
    }

    private string DecryptRefreshToken(byte[] encryptedRefreshToken)
    {
        var unprotectedBytes = ProtectedData.Unprotect(encryptedRefreshToken, null, DataProtectionScope.CurrentUser);
        return Encoding.UTF8.GetString(unprotectedBytes);
    }

    public async Task WriteCharacterAuthData(int eveCharacterId, AuthorizedToken token, EveSsoCharacterInfo info)
    {
        var character = await context.Characters.FirstOrDefaultAsync(c => c.EveCharacterId == eveCharacterId);

        if (character == null)
        {
            character = new Character
            {
                EveCharacterId = info.CharacterId,
                Name = info.CharacterName,
                CharacterOwnerHash = info.CharacterOwnerHash,
                AllianceId = info.AllianceId,
                CorporationId = info.CorporationId,
                FactionId = info.FactionId,
            };

            context.Characters.Add(character);
        }
        else
        {
            character.Name = info.CharacterName;
            character.CharacterOwnerHash = info.CharacterOwnerHash;
            character.CorporationId = info.CorporationId;
            character.FactionId = info.FactionId;
            character.AllianceId = info.AllianceId;
        }

        await context.SaveChangesAsync();

        var encryptedRefreshToken = EncryptRefreshToken(token.RefreshToken);

        var authToken = await context.CharacterAuthTokens
            .Include(t => t.Scopes)
            .FirstOrDefaultAsync(t => t.Character.EveCharacterId == eveCharacterId);

        if (authToken == null)
        {
            authToken = new CharacterAuthToken
            {
                CharacterId = character.Id,
                EncryptedRefreshToken = encryptedRefreshToken,
                ExpiresOn = token.ExpiresOn
            };

            context.CharacterAuthTokens.Add(authToken);
        }
        else
        {
            authToken.EncryptedRefreshToken = encryptedRefreshToken;
            authToken.ExpiresOn = token.ExpiresOn;

            // Clear existing scopes
            authToken.ClearScopes();
        }

        // Add new scopes
        foreach (var scope in token.Scopes)
        {
            authToken.AddScope(scope);
        }

        await context.SaveChangesAsync();
    }

    private byte[] EncryptRefreshToken(string refreshToken)
    {
        var refreshTokenBytes = Encoding.UTF8.GetBytes(refreshToken);
        return ProtectedData.Protect(refreshTokenBytes, null, DataProtectionScope.CurrentUser);
    }

    public async Task<EveSsoCharacterInfo?> GetCharacterInfo(int eveCharacterId)
    {
        var character = await context.Characters
            .FirstOrDefaultAsync(c => c.EveCharacterId == eveCharacterId);

        if (character == null)
            return null;

        return new EveSsoCharacterInfo
        {
            CharacterId = character.EveCharacterId,
            CharacterName = character.Name,
            CharacterOwnerHash = character.CharacterOwnerHash,
            AllianceId = character.AllianceId,
            CorporationId = character.CorporationId,
            FactionId = character.FactionId,
        };
    }

    public async Task RemoveCharacter(int characterId)
    {
        var character = context.Characters.FirstOrDefault(f => f.EveCharacterId == characterId);
        if (character is null) return;
        context.Remove(character);
        await context.SaveChangesAsync();
    }
}