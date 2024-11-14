namespace EveStationJanitor.Core.Eve;

public class Standings(Skills skills)
{
    private readonly Dictionary<CorporationId, (Standing Base, Standing Effective)> _standings = [];

    public void AddStanding(CorporationId corporationId, Standing unmodifiedStanding, bool isCriminal = false)
    {
        Standing effectiveStanding;

        // Determine the skill to apply based on the standing value and whether it's a criminal entity
        if (unmodifiedStanding >= 0)
        {
            var skillLevel = isCriminal
                ? skills.CriminalConnections
                : skills.Connections;

            effectiveStanding = CalculateEffectiveStanding(unmodifiedStanding, skillLevel, 0.04d); // 4% modifier for Connections/Criminal Connections
        }
        else
        {
            var skillLevel = skills.Diplomacy;
            effectiveStanding = CalculateEffectiveStanding(unmodifiedStanding, skillLevel, 0.04d); // 4% modifier for Diplomacy
        }

        _standings[corporationId] = (unmodifiedStanding, effectiveStanding);
    }

    public Standing GetEffectiveStanding(CorporationId corporationId)
    {
        return _standings.TryGetValue(corporationId, out var standings) 
            ? standings.Effective
            : 0.0;
    }

    private static Standing CalculateEffectiveStanding(Standing unmodifiedStanding, SkillLevel skillLevel, double modifier)
    {
        const Standing maxStanding = 10;
        
        return unmodifiedStanding 
               + (maxStanding - unmodifiedStanding)
               * (modifier * skillLevel);
    }
}
