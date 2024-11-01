using System.Collections.Frozen;
using EveStationJanitor.Core.DataAccess.Entities;

namespace EveStationJanitor.Core.Eve;

public class CloneImplants
{
    private static readonly FrozenDictionary<int, decimal> ImplantReprocessingBonuses = new Dictionary<int, decimal> {
        { 27175, 0.01m }, // Zainou 'Beancounter' Reprocessing RX-801
        { 27169, 0.02m }, // Zainou 'Beancounter' Reprocessing RX-802
        { 27174, 0.04m }  // Zainou 'Beancounter' Reprocessing RX-804
    }.ToFrozenDictionary();
    
    private readonly List<ItemType> _implants = [];
    private Lazy<decimal> _implantReprocessingEfficiency;

    public CloneImplants()
    {
        // TODO - If we introduce more bonus values in this class then maybe extract them into a CloneImplantBonuses class
        _implantReprocessingEfficiency = new Lazy<decimal>(CalculateCloneImplantBonusReprocessingEfficiency);
    }
    
    public IReadOnlyCollection<ItemType> Implants => _implants;
    
    public decimal ImplantReprocessingEfficiency => _implantReprocessingEfficiency.Value;

    public void AddImplants(IEnumerable<ItemType> implants)
    {
        _implants.AddRange(implants);
        _implantReprocessingEfficiency = new Lazy<decimal>(CalculateCloneImplantBonusReprocessingEfficiency);
    }
    
    public void AddImplant(ItemType implant)
    {
        _implants.Add(implant);
        _implantReprocessingEfficiency = new Lazy<decimal>(CalculateCloneImplantBonusReprocessingEfficiency);
    }
    
    private decimal CalculateCloneImplantBonusReprocessingEfficiency()
    {
        if (Implants.Count == 0)
        {
            return 0.0m;
        }

        foreach (var implant in Implants)
        {
            // This assumes there's only one reprocessing implant pluggable at any one time...
            if (ImplantReprocessingBonuses.TryGetValue(implant.Id, out var bonus))
            {
                return bonus;
            }
        }

        return 0.0m;
    }
}
