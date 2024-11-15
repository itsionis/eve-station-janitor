namespace EveStationJanitor.Core.Eve.Formula;

public static class StandingsFormula
{
    private const StandingsModifier ConnectionsModifier = 0.04d;
    private const StandingsModifier CriminalConnectionsModifier = 0.04d;
    private const StandingsModifier DiplomacyModifier = 0.04d;

    /// <summary>
    /// Computes the effective standing between the player and a non-criminal NPC entity.
    /// </summary>
    /// <remarks>
    /// Calculation derived from https://wiki.eveuniversity.org/NPC_standings#Skills
    /// </remarks>
    public static Standing EffectiveStanding(
        Standing unmodifiedStanding,
        SkillLevel connectionsSkill,
        SkillLevel diplomacySkill)
    {
        if (unmodifiedStanding < 0)
        {
            // Negative standings are only affected by the diplomacy skill
            return unmodifiedStanding + ((10 - unmodifiedStanding) * (DiplomacyModifier * diplomacySkill));
        }

        return unmodifiedStanding + ((10 - unmodifiedStanding) * (ConnectionsModifier * connectionsSkill));
    }

    /// <summary>
    /// Computes the effective standing between the player and a criminal NPC entity.
    /// </summary>
    /// <remarks>
    /// Calculation derived from https://wiki.eveuniversity.org/NPC_standings#Skills
    /// </remarks>
    public static Standing EffectiveCriminalStanding(
        Standing unmodifiedStanding,
        SkillLevel criminalConnectionsSkill,
        SkillLevel diplomacySkill)
    {
        if (unmodifiedStanding < 0)
        {
            // Negative standings are only affected by the diplomacy skill
            return unmodifiedStanding + ((10 - unmodifiedStanding) * (DiplomacyModifier * diplomacySkill));
        }

        return unmodifiedStanding + ((10 - unmodifiedStanding) * (CriminalConnectionsModifier * criminalConnectionsSkill));
    }
}