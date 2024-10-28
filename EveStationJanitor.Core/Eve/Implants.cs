using EveStationJanitor.Core.DataAccess.Entities;

namespace EveStationJanitor.Core.Eve;

public class CloneImplants
{
    private List<ItemType> _implants = [];

    public IReadOnlyCollection<ItemType> Implants => _implants;

    public void AddImplants(IEnumerable<ItemType> implants)
    {
        _implants.AddRange(implants);
    }
    public void AddImplant(ItemType implant)
    {
        _implants.Add(implant);
    }
}
