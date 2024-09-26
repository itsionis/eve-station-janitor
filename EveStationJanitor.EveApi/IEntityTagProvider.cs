namespace EveStationJanitor.EveApi;

public interface IEntityTagProvider
{
    string? GetEntityTag(string key);

    void SetEntityTag(string key, string tag);
}
