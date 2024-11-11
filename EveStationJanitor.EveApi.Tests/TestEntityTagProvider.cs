using System.Net.Http.Headers;

namespace EveStationJanitor.EveApi.Tests;

public class TestEntityTagProvider :IEntityTagProvider
{
    private readonly Dictionary<string, EntityTagHeaderValue> _headers = [];
    
    public string? GetEntityTag(string key)
    {
        return _headers.TryGetValue(key, out var value) ? value.Tag : null;
    }

    public void SetEntityTag(string key, string tag)
    {
        _headers[key] = new EntityTagHeaderValue(tag);
    }
}
