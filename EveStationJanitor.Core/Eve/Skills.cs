using EveStationJanitor.EveApi.Character.Objects;

namespace EveStationJanitor.Core.Eve;

public class Skills
{
    private readonly Dictionary<int, SkillLevel> _skillLevels = [];

    public Skills()
    {
    }

    public Skills(ApiCharacterSkills skills)
    {
        foreach (var skill in skills.Skills)
        {
            SetSkillLevel(skill.SkillId, skill.ActiveSkillLevel);
        }
    }

    private SkillLevel GetSkillLevel(int skillId)
    {
        return _skillLevels.TryGetValue(skillId, out var level) ? level : 0;
    }

    private void SetSkillLevel(int skillId, SkillLevel skillLevel)
    {
        if (skillLevel is < 0 or > 5) throw new ArgumentOutOfRangeException(nameof(skillLevel));
        _skillLevels[skillId] = skillLevel;
    }

    // Trade
    public SkillLevel Accounting => GetSkillLevel(16622);

    // Ore Processing
    public SkillLevel SimpleOreProcessing
    {
        get => GetSkillLevel(60377);
        set => SetSkillLevel(60377, value);
    }

    public SkillLevel AbyssalOreProcessing => GetSkillLevel(60381);
    public SkillLevel CoherentOreProcessing => GetSkillLevel(60378);
    public SkillLevel ComplexOreProcessing => GetSkillLevel(60380);
    public SkillLevel VariegatedOreProcessing => GetSkillLevel(60379);
    public SkillLevel MercoxitOreProcessing => GetSkillLevel(12189);
    public SkillLevel IceProcessing => GetSkillLevel(18025);
    public SkillLevel CommonMoonOreProcessing => GetSkillLevel(46153);
    public SkillLevel ExceptionalMoonOreProcessing => GetSkillLevel(46156);
    public SkillLevel RareMoonOreProcessing => GetSkillLevel(46155);
    public SkillLevel UbiquitousMoonOreProcessing => GetSkillLevel(46152);
    public SkillLevel UncommonMoonOreProcessing => GetSkillLevel(46154);

    // Reprocessing
    public SkillLevel Reprocessing
    {
        get => GetSkillLevel(3385);
        set => SetSkillLevel(3385,value);
    }

    public SkillLevel ReprocessingEfficiency
    {
        get => GetSkillLevel(3389);
        set => SetSkillLevel(3389,value);
    }

    public SkillLevel ScrapmetalProcessing
    {
        get => GetSkillLevel(12196);
        set => SetSkillLevel(12196, value);
    }

    // Social
    public SkillLevel Social
    {
        get => GetSkillLevel(3355);
        set => SetSkillLevel(3355, value);
    }

    public SkillLevel Diplomacy
    {
        get => GetSkillLevel(3357);
        set => SetSkillLevel(3357, value);
    }

    public SkillLevel Connections
    {
        get => GetSkillLevel(3359);
        set => SetSkillLevel(3359, value);
    }

    public SkillLevel CriminalConnections => GetSkillLevel(3361);
}