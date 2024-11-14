using EveStationJanitor.Core.DataAccess;
using EveStationJanitor.Core.DataAccess.Entities;
using EveStationJanitor.EveApi;

namespace EveStationJanitor.Core;

internal class EntityTagProvider : IEntityTagProvider
{
    private readonly object _lock = new();

    private readonly AppDbContext _context;

    public EntityTagProvider(AppDbContext context)
    {
        _context = context;
    }

    public string? GetEntityTag(string key)
    {
        lock (_lock)
        {
            return _context.EntityTags.FirstOrDefault(tag => tag.Key == key)?.Tag;
        }
    }

    public void SetEntityTag(string key, string tag)
    {
        lock (_lock)
        {
            var maybeEntityTag = _context.EntityTags.FirstOrDefault(entityTag => entityTag.Key == key);
            if (maybeEntityTag is not null)
            {
                maybeEntityTag.Tag = tag;
            }
            else
            {
                _context.EntityTags.Add(new EntityTag { Key = key, Tag = tag });
            }

            _context.SaveChanges();
        }
    }
}
