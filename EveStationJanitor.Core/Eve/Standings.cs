namespace EveStationJanitor.Core.Eve;

public class Standings(Skills skills)
{
    private readonly Dictionary<int, double> _effectiveStandings = [];

    public void AddStanding(int corporationId, double unmodifiedStanding, bool isCriminal = false)
    {
        double effectiveStanding;

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

        // Store the effective standing in the dictionary
        _effectiveStandings[corporationId] = effectiveStanding;
    }

    public double GetStanding(int corporationId)
    {
        if (_effectiveStandings.TryGetValue(corporationId, out var standing))
        {
            return standing;
        }

        // Default to 0 if no standing found for the corporation
        return 0;
    }

    private static double CalculateEffectiveStanding(double unmodifiedStanding, int skillLevel, double modifier)
    {
        return unmodifiedStanding + (10 - unmodifiedStanding) * (modifier * skillLevel);
    }
}
