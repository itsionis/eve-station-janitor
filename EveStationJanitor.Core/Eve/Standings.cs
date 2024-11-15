using EveStationJanitor.Core.Eve.Formula;

namespace EveStationJanitor.Core.Eve;

public class Standings(Skills skills)
{
    private readonly Dictionary<CorporationId, (Standing Base, Standing Effective)> _standings = [];

    public void AddStanding(CorporationId corporationId, Standing unmodifiedStanding, bool isCriminal = false)
    {
        var effectiveStanding = isCriminal
                ? StandingsFormula.EffectiveCriminalStanding(unmodifiedStanding, skills.CriminalConnections, skills.Diplomacy)
                : StandingsFormula.EffectiveStanding(unmodifiedStanding, skills.Connections, skills.Diplomacy);

        _standings[corporationId] = (unmodifiedStanding, effectiveStanding);
    }

    public Standing GetEffectiveStanding(CorporationId corporationId)
    {
        return _standings.TryGetValue(corporationId, out var standings) 
            ? standings.Effective
            : 0.0;
    }
}
