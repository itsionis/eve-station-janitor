using EveStationJanitor.Core.DataAccess;
using EveStationJanitor.Core.DataAccess.Entities;

namespace EveStationJanitor.Core;

public class OreReprocessing
{
    private readonly AppDbContext _context;
    private readonly Skills _skills;

    private readonly Dictionary<int, int> _oreReprocessingSkillLevels = [];
    private readonly Dictionary<int, decimal> _oreYields = [];

    public OreReprocessing(AppDbContext context, Skills skills)
    {
        _context = context;
        _skills = skills;

        InitialiseOres();
    }

    private void InitialiseOres()
    {
        var simpleOreProcessingSkillLevel = _skills.SimpleOreProcessing;
        RegisterOre(["Plagioclase", "Pyroxeres", "Scordite", "Veldspar", "Mordunium"], simpleOreProcessingSkillLevel);
        RegisterOre(["Azure Plagioclase", "Solid Pyroxeres", "Condensed Scordite", "Concentrated Veldspar", "Plum Mordunium"], simpleOreProcessingSkillLevel, 0.05m);
        RegisterOre(["Rich Plagioclase", "Viscous Pyroxeres", "Massive Scordite", "Dense Veldspar", "Prize Mordunium"], simpleOreProcessingSkillLevel, 0.10m);
        RegisterOre(["Sparkling Plagioclase", "Opulent Pyroxeres", "Glossy Scordite", "Stable Veldspar", "Plunder Mordunium"], simpleOreProcessingSkillLevel, 0.15m);

        var abyssalOreProcessingSkillLevel = _skills.AbyssalOreProcessing;
        RegisterOre(["Bezdnacine", "Rakovene", "Talassonite"], abyssalOreProcessingSkillLevel);
        RegisterOre(["Abyssal Bezdnacine", "Abyssal Rakovene", "Abyssal Talassonite"], abyssalOreProcessingSkillLevel, 0.05m);
        RegisterOre(["Hadal Bezdnacine", "Hadal Rakovene", "Hadal Talassonite"], abyssalOreProcessingSkillLevel, 0.10m);

        var coherentOreProcessingSkillLevel = _skills.CoherentOreProcessing;
        RegisterOre(["Hedbergite", "Hemorphite", "Jaspet", "Kernite", "Omber", "Ytirium"], coherentOreProcessingSkillLevel);
        RegisterOre(["Vitric Hedbergite", "Vivid Hemorphite", "Pure Jaspet", "Luminous Kernite", "Silvery Omber"], coherentOreProcessingSkillLevel, 0.05m);
        RegisterOre(["Glazed Hedbergite", "Radiant Hemorphite", "Pristine Jaspet", "Fiery Kernite", "Golden Omber"], coherentOreProcessingSkillLevel, 0.10m);
        RegisterOre(["Lustrous Hedbergite", "Scintillating Hemorphite ", "Immaculate Jaspet", "Resplendant Kernite", "Platinoid Omber"], coherentOreProcessingSkillLevel, 0.15m);

        var complexOreProcessingSkillLevel = _skills.ComplexOreProcessing;
        RegisterOre(["Arkonor", "Bistot", "Spodumain", "Eifyrium", "Ducinium"], complexOreProcessingSkillLevel);
        RegisterOre(["Crimson Arkonor", "Triclinic Bistot", "Bright Spodumain", "Doped Eifyrium", "Noble Ducinium"], complexOreProcessingSkillLevel, 0.05m);
        RegisterOre(["Prime Arkonor", "Monoclinic Bistot", "Gleaming Spodumain", "Boosted Eifyrium", "Royal Ducinium"], complexOreProcessingSkillLevel, 0.10m);
        RegisterOre(["Flawless Arkonor", "Cubic Bistot", "Dazzling Spodumain", "Augmented Eifyrium", "Imperial Ducinium"], complexOreProcessingSkillLevel, 0.15m);

        var variegatedOreProcessingSkillLevel = _skills.VariegatedOreProcessing;
        RegisterOre(["Crokite", "Dark Ochre", "Gneiss"], variegatedOreProcessingSkillLevel);
        RegisterOre(["Sharp Crokite", "Onyx Ochre", "Iridescent Gneiss"], variegatedOreProcessingSkillLevel, 0.05m);
        RegisterOre(["Crystalline Crokite", "Obsidian Ochre", "Prismatic Gneiss"], variegatedOreProcessingSkillLevel, 0.10m);
        RegisterOre(["Pellucid Crokite", "Pellucid Ochre", "Brilliant Gneiss"], variegatedOreProcessingSkillLevel, 0.15m);

        var mercoxitOreProcessingSkillLevel = _skills.MercoxitOreProcessing; 
        RegisterOre(["Mercoxit"], mercoxitOreProcessingSkillLevel);
        RegisterOre(["Magma Mercoxit"], mercoxitOreProcessingSkillLevel, 0.05m);
        RegisterOre(["Vitreous Mercoxit"], mercoxitOreProcessingSkillLevel, 0.10m);

        var commonMoonOreProcessingSkillLevel = _skills.CommonMoonOreProcessing;
        RegisterOre(["Cobaltite", "Euxenite", "Titanite", "Scheelite"], commonMoonOreProcessingSkillLevel);
        RegisterOre(["Copious Cobaltite", "Copious Euxenite", "Copious Titanite", "Copious Scheelite"], commonMoonOreProcessingSkillLevel, 0.15m);
        RegisterOre(["Twinkling Cobaltite", "Twinkling Euxenite", "Twinkling Titanite", "Twinkling Scheelite"], commonMoonOreProcessingSkillLevel, 1m);

        var exceptionalMoonOreProcessingSkillLevel = _skills.ExceptionalMoonOreProcessing;
        RegisterOre(["Xenotime", "Monazite", "Loparite", "Ytterbite"], exceptionalMoonOreProcessingSkillLevel);
        RegisterOre(["Bountiful Xenotime", "Bountiful Monazite", "Bountiful Loparite", "Bountiful Ytterbite"], exceptionalMoonOreProcessingSkillLevel, 0.15m);
        RegisterOre(["Shining Xenotime", "Shining Monazite", "Shining Loparite", "Shining Ytterbite"], exceptionalMoonOreProcessingSkillLevel, 1m);

        var rareMoonOreProcessingSkillLevel = _skills.RareMoonOreProcessing; 
        RegisterOre(["Carnotite", "Zircon", "Pollucite", "Cinnabar"], rareMoonOreProcessingSkillLevel);
        RegisterOre(["Replete Carnotite", "Replete Zircon", "Replete Pollucite", "Replete Cinnabar"], rareMoonOreProcessingSkillLevel, 0.15m);
        RegisterOre(["Glowing Carnotite", "Glowing Zircon", "Glowing Pollucite", "Glowing Cinnabar"], rareMoonOreProcessingSkillLevel, 1m);

        var ubiquitousMoonOreProcessingSkillLevel = _skills.UbiquitousMoonOreProcessing;
        RegisterOre(["Zeolites", "Sylvite", "Bitumens", "Coesite"], ubiquitousMoonOreProcessingSkillLevel);
        RegisterOre(["Brimful Zeolites", "Brimful Sylvite", "Brimful Bitumens", "Brimful Coesite"], ubiquitousMoonOreProcessingSkillLevel, 0.15m);
        RegisterOre(["Glistening Zeolites", "Glistening Sylvite", "Glistening Bitumens", "Glistening Coesite"], ubiquitousMoonOreProcessingSkillLevel, 1m);

        var uncommonMoonOreProcessingSkillLevel = _skills.UncommonMoonOreProcessing;
        RegisterOre(["Otavite", "Sperrylite", "Vanadinite", "Chromite"], uncommonMoonOreProcessingSkillLevel);
        RegisterOre(["Lavish Otavite", "Lavish Sperrylite", "Lavish Vanadinite", "Lavish Chromite"], uncommonMoonOreProcessingSkillLevel, 0.15m);
        RegisterOre(["Shimmering Otavite", "Shimmering Sperrylite", "Shimmering Vanadinite", "Shimmering Chromite"], uncommonMoonOreProcessingSkillLevel, 1m);

        var iceProcessingSkillLevel = _skills.IceProcessing;
        RegisterOre(["Glare Crust", "Dark Glitter", "Gelidus", "Krystallos", "Clear Icicle", "White Glaze", "Blue Ice", "Glacial Mass"], iceProcessingSkillLevel);
    }

    private void RegisterOre(List<string> oreNames, int skillLevel, decimal extraYield = 0)
    {
        foreach (var item in _context.ItemTypes.Where(item => oreNames.Contains(item.Name)))
        {
            _oreReprocessingSkillLevels[item.Id] = skillLevel;
            _oreYields[item.Id] = extraYield;

            // Register compressed and batch compressed variants
            RegisterCompressedVariants(item, skillLevel, extraYield);
        }
    }

    private void RegisterCompressedVariants(ItemType item, int skillLevel, decimal extraYield)
    {
        var compressed = $"Compressed {item.Name}";
        var compressedOre = _context.ItemTypes.FirstOrDefault(item => item.Name == compressed);
        if (compressedOre != null)
        {
            _oreReprocessingSkillLevels[compressedOre.Id] = skillLevel;
            _oreYields[compressedOre.Id] = extraYield;
        }

        var batchCompressed = $"Batch Compressed {item.Name}";
        var batchCompressedOre = _context.ItemTypes.FirstOrDefault(item => item.Name == batchCompressed);
        if (batchCompressedOre != null)
        {
            _oreReprocessingSkillLevels[batchCompressedOre.Id] = skillLevel;
            _oreYields[batchCompressedOre.Id] = extraYield;
        }
    }

    public decimal GetOreYield(int typeId)
    {
        return _oreYields.TryGetValue(typeId, out var yield) ? yield : 0m;
    }

    public int? GetOreReprocessingSkillLevel(int typeId)
    {
        if(_oreReprocessingSkillLevels.TryGetValue(typeId, out var skillLevel))
        {
            return skillLevel;
        }
        return null;
    }
}
