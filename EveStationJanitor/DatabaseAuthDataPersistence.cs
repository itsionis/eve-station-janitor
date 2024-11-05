using EveStationJanitor.Authentication;
using EveStationJanitor.Core.DataAccess.Entities;
using EveStationJanitor.Core.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace EveStationJanitor;

public class DatabaseAuthDataPersistence(AppDbContext context, IAuthenticationClient authenticationClient) : IAuthenticationDataProvider
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

    private static string DecryptRefreshToken(byte[] encryptedRefreshToken)
    {
        var unprotectedBytes = ProtectedData.Unprotect(encryptedRefreshToken, null, DataProtectionScope.CurrentUser);
        return Encoding.UTF8.GetString(unprotectedBytes);
    }

    public async Task SaveCharacterAuthData(AuthorizedToken token, EveSsoCharacterInfo info)
    {
        var character = await context.Characters.FirstOrDefaultAsync(c => c.EveCharacterId == info.CharacterId);
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
            .FirstOrDefaultAsync(t => t.Character.EveCharacterId == info.CharacterId);

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

    private static byte[] EncryptRefreshToken(string refreshToken)
    {
        var refreshTokenBytes = Encoding.UTF8.GetBytes(refreshToken);
        return ProtectedData.Protect(refreshTokenBytes, null, DataProtectionScope.CurrentUser);
    }

    public async Task DeleteCharacter(int characterId)
    {
        var character = context.Characters.FirstOrDefault(f => f.EveCharacterId == characterId);
        if (character is null) return;
        context.Remove(character);
        await context.SaveChangesAsync();
    }

    public async Task<bool> EnsureCharacterAuthenticated(int eveCharacterId)
    {
        var characterTokens = await GetAuthTokens(eveCharacterId);
        if (characterTokens is null)
        {
            // Could not acquire authentication tokens for character. Start a new auth flow for that specific character.
            var authenticationResult = await authenticationClient.Authenticate(eveCharacterId);
            if (authenticationResult is null)
            {
                return false;
            }
            
            var (tokens, character) = authenticationResult.Value;
            await SaveCharacterAuthData(tokens, character);
        }
        else
        {
            // Character has tokens, give them a refresh just in-case they're expired...
            var refreshResult = await authenticationClient.Refresh(characterTokens);
            if (refreshResult is null)
            {
                return false;
            }
            
            var (tokens, character) = refreshResult.Value;
            await SaveCharacterAuthData(tokens, character);
        }

        return true;
    }

    public async Task<int?> AuthenticateNewCharacter()
    {
        var authenticationResult = await authenticationClient.Authenticate();
        if (authenticationResult is null)
        {
            return null;
        }
        
        var (authorizedToken, characterInfo) = authenticationResult.Value;
        await SaveCharacterAuthData(authorizedToken, characterInfo);
        return characterInfo.CharacterId;
    }
}
