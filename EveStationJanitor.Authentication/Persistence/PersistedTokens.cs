using EveStationJanitor.Authentication.Tokens;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text.Json;

namespace EveStationJanitor.Authentication.Persistence;

internal sealed class PersistedTokens : ITokenPersistence
{
    private readonly string _tokenPath;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public PersistedTokens(
        TokenPath tokenPath,
        [FromKeyedServices(JsonSourceGeneratorContext.ServiceKey)] JsonSerializerOptions jsonOptions)
    {
        _jsonSerializerOptions = jsonOptions;
        _tokenPath = tokenPath.Path;
    }

    [SupportedOSPlatform("windows")]
    public AuthorizedToken? ReadTokens()
    {
        if (!File.Exists(_tokenPath))
        {
            return null;
        }

        try
        {
            var protectedBytes = File.ReadAllBytes(_tokenPath);
            var unprotectedBytes = ProtectedData.Unprotect(protectedBytes, null, DataProtectionScope.CurrentUser);
            var json = Encoding.UTF8.GetString(unprotectedBytes);
            return JsonSerializer.Deserialize<AuthorizedToken>(json, _jsonSerializerOptions);
        }
        catch
        {
            return null;
        }
    }

    [SupportedOSPlatform("windows")]
    public void WriteTokens(AuthorizedToken token)
    {
        var directory = Path.GetDirectoryName(_tokenPath);
        Directory.CreateDirectory(directory!);

        var json = JsonSerializer.Serialize(token, _jsonSerializerOptions);
        var unprotectedBytes = Encoding.UTF8.GetBytes(json);
        var protectedBytes = ProtectedData.Protect(unprotectedBytes, null, DataProtectionScope.CurrentUser);
        File.WriteAllBytes(_tokenPath, protectedBytes);
    }
}
