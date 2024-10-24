using EveStationJanitor.EveApi.Character.Objects;

namespace EveStationJanitor.Core;

public class Skills
{
    private readonly Dictionary<int, int> _skillLevels = [];

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
    public int SimpleOreProcessing => GetSkillLevel(60377);
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
    public int Reprocessing => GetSkillLevel(3385);
    public int ReprocessingEfficiency => GetSkillLevel(3389);
    public int ScrapMetalProcessing => GetSkillLevel(12196);

    // Social
    public int Social => GetSkillLevel(3355); 
    public int Diplomacy => GetSkillLevel(3357);
    public int Connections => GetSkillLevel(3359); 
    public int CriminalConnections => GetSkillLevel(3361); 

}
