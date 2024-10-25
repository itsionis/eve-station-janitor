using EveStationJanitor.EveApi.Character.Objects;

namespace EveStationJanitor.Core.Eve;

public class Skills
{
    private readonly Dictionary<int, int> _skillLevels = [];

    public Skills()
    {
    }

    public Skills(ApiCharacterSkills skills)
    {
        foreach (var skill in skills.Skills)
        {
            _skillLevels[skill.SkillId] = skill.ActiveSkillLevel;
        }
    }

    private int GetSkillLevel(int skillId)
    {
        if (_skillLevels.TryGetValue(skillId, out var level))
        {
            return level;
        }
        else
        {
            return 0;
        }
    }

    // Trade
    public int Accounting => GetSkillLevel(16622);

    // Ore Processing
    public int SimpleOreProcessing
    {
        get => GetSkillLevel(60377);
        set => _skillLevels[60377] = value;
    }

    public int AbyssalOreProcessing => GetSkillLevel(60381);
    public int CoherentOreProcessing => GetSkillLevel(60378);
    public int ComplexOreProcessing => GetSkillLevel(60380);
    public int VariegatedOreProcessing => GetSkillLevel(60379);
    public int MercoxitOreProcessing => GetSkillLevel(12189);
    public int IceProcessing => GetSkillLevel(18025);
    public int CommonMoonOreProcessing => GetSkillLevel(46153);
    public int ExceptionalMoonOreProcessing => GetSkillLevel(46156);
    public int RareMoonOreProcessing => GetSkillLevel(46155);
    public int UbiquitousMoonOreProcessing => GetSkillLevel(46152);
    public int UncommonMoonOreProcessing => GetSkillLevel(46154);

    // Reprocessing
    public int Reprocessing
    {
        get => GetSkillLevel(3385);
        set => _skillLevels[3385] = value;
    }

    public int ReprocessingEfficiency
    {
        get => GetSkillLevel(3389);
        set => _skillLevels[3389] = value;
    }

    public int ScrapmetalProcessing
    {
        get => GetSkillLevel(12196);
        set => _skillLevels[12196] = value;
    }

    // Social
    public int Social
    {
        get => GetSkillLevel(3355);
        set => _skillLevels[3355] = value;
    }

    public int Diplomacy
    {
        get => GetSkillLevel(3357);
        set => _skillLevels[3357] = value;
    }

    public int Connections
    {
        get => GetSkillLevel(3359);
        set => _skillLevels[3359] = value;
    }

    public int CriminalConnections => GetSkillLevel(3361);
}