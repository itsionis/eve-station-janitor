using EveStationJanitor.Core.Eve;

namespace EveStationJanitor.Core;

using OreId = int;
using SkillId = int;

public class OreReprocessing
{
    private readonly Skills _skills;

    private readonly Dictionary<OreId, SkillId> _oreReprocessingSkillLevels = [];

    public OreReprocessing(Skills skills)
    {
        _skills = skills;
        InitialiseOres();
    }

    private void InitialiseOres()
    {
        RegisterOre([
            18, // Plagioclase
            62528, // Compressed Plagioclase
            28422, // Batch Compressed Plagioclase
            1224, // Pyroxeres
            62524, // Compressed Pyroxeres
            28424, // Batch Compressed Pyroxeres
            1228, // Scordite
            62520, // Compressed Scordite
            28429, // Batch Compressed Scordite
            1230, // Veldspar
            62516, // Compressed Veldspar
            28432, // Batch Compressed Veldspar
            74521, // Mordunium
            75275, // Compressed Mordunium
            17455, // Azure Plagioclase
            62529, // Compressed Azure Plagioclase
            28421, // Batch Compressed Azure Plagioclase
            17459, // Solid Pyroxeres
            62525, // Compressed Solid Pyroxeres
            28425, // Batch Compressed Solid Pyroxeres
            17463, // Condensed Scordite
            62521, // Compressed Condensed Scordite
            28427, // Batch Compressed Condensed Scordite
            17470, // Concentrated Veldspar
            62517, // Compressed Concentrated Veldspar
            28430, // Batch Compressed Concentrated Veldspar
            74522, // Plum Mordunium
            75276, // Compressed Plum Mordunium
            17456, // Rich Plagioclase
            62530, // Compressed Rich Plagioclase
            28423, // Batch Compressed Rich Plagioclase
            17460, // Viscous Pyroxeres
            62526, // Compressed Viscous Pyroxeres
            28426, // Batch Compressed Viscous Pyroxeres
            17464, // Massive Scordite
            62522, // Compressed Massive Scordite
            28428, // Batch Compressed Massive Scordite
            17471, // Dense Veldspar
            62518, // Compressed Dense Veldspar
            28431, // Batch Compressed Dense Veldspar
            74523, // Prize Mordunium
            75277, // Compressed Prize Mordunium
            46685, // Sparkling Plagioclase
            62531, // Compressed Sparkling Plagioclase
            46701, // Batch Compressed Sparkling Plagioclase
            46686, // Opulent Pyroxeres
            62527, // Compressed Opulent Pyroxeres
            46702, // Batch Compressed Opulent Pyroxeres
            46687, // Glossy Scordite
            62523, // Compressed Glossy Scordite
            46703, // Batch Compressed Glossy Scordite
            46689, // Stable Veldspar
            62519, // Compressed Stable Veldspar
            46705, // Batch Compressed Stable Veldspar
            74524, // Plunder Mordunium
            75278 // Compressed Plunder Mordunium
        ], _skills.SimpleOreProcessing);

        RegisterOre([
            52316, // Bezdnacine
            62576, // Compressed Bezdnacine
            52315, // Rakovene
            62579, // Compressed Rakovene
            52306, // Talassonite
            62582, // Compressed Talassonite
            56627, // Abyssal Bezdnacine
            62577, // Compressed Abyssal Bezdnacine
            56629, // Abyssal Rakovene
            62580, // Compressed Abyssal Rakovene
            56625, // Abyssal Talassonite
            62583, // Compressed Abyssal Talassonite
            56628, // Hadal Bezdnacine
            62578, // Compressed Hadal Bezdnacine
            56630, // Hadal Rakovene
            62581, // Compressed Hadal Rakovene
            56626, // Hadal Talassonite
            62584 // Compressed Hadal Talassonite
        ], _skills.AbyssalOreProcessing);

        RegisterOre([
            21, // Hedbergite
            62548, // Compressed Hedbergite
            28401, // Batch Compressed Hedbergite
            1231, // Hemorphite
            62544, // Compressed Hemorphite
            28403, // Batch Compressed Hemorphite
            1226, // Jaspet
            62540, // Compressed Jaspet
            28406, // Batch Compressed Jaspet
            20, // Kernite
            62536, // Compressed Kernite
            28410, // Batch Compressed Kernite
            1227, // Omber
            62532, // Compressed Omber
            28416, // Batch Compressed Omber
            74525, // Ytirium
            75279, // Compressed Ytirium
            17440, // Vitric Hedbergite
            62549, // Compressed Vitric Hedbergite
            28402, // Batch Compressed Vitric Hedbergite
            17444, // Vivid Hemorphite
            62545, // Compressed Vivid Hemorphite
            28405, // Batch Compressed Vivid Hemorphite
            17448, // Pure Jaspet
            62541, // Compressed Pure Jaspet
            28408, // Batch Compressed Pure Jaspet
            17452, // Luminous Kernite
            62537, // Compressed Luminous Kernite
            28411, // Batch Compressed Luminous Kernite
            17867, // Silvery Omber
            62533, // Compressed Silvery Omber
            28417, // Batch Compressed Silvery Omber
            17441, // Glazed Hedbergite
            62550, // Compressed Glazed Hedbergite
            28400, // Batch Compressed Glazed Hedbergite
            17445, // Radiant Hemorphite
            62546, // Compressed Radiant Hemorphite
            28404, // Batch Compressed Radiant Hemorphite
            17449, // Pristine Jaspet
            62542, // Compressed Pristine Jaspet
            28407, // Batch Compressed Pristine Jaspet
            17453, // Fiery Kernite
            62538, // Compressed Fiery Kernite
            28409, // Batch Compressed Fiery Kernite
            17868, // Golden Omber
            62534, // Compressed Golden Omber
            28415, // Batch Compressed Golden Omber
            46680, // Lustrous Hedbergite
            62551, // Compressed Lustrous Hedbergite
            46696, // Batch Compressed Lustrous Hedbergite
            46681, // Scintillating Hemorphite
            62547, // Compressed Scintillating Hemorphite
            46697, // Batch Compressed Scintillating Hemorphite
            46682, // Immaculate Jaspet
            62543, // Compressed Immaculate Jaspet
            46698, // Batch Compressed Immaculate Jaspet
            46683, // Resplendant Kernite
            62539, // Compressed Resplendant Kernite
            46699, // Batch Compressed Resplendant Kernite
            46684, // Platinoid Omber
            62535, // Compressed Platinoid Omber
            46700 // Batch Compressed Platinoid Omber
        ], _skills.CoherentOreProcessing);

        RegisterOre([
            22, // Arkonor
            62568, // Compressed Arkonor
            28367, // Batch Compressed Arkonor
            1223, // Bistot
            62564, // Compressed Bistot
            28388, // Batch Compressed Bistot
            19, // Spodumain
            62572, // Compressed Spodumain
            28420, // Batch Compressed Spodumain
            74529, // Eifyrium
            75283, // Compressed Eifyrium
            74533, // Ducinium
            75287, // Compressed Ducinium
            17425, // Crimson Arkonor
            62569, // Compressed Crimson Arkonor
            28385, // Batch Compressed Crimson Arkonor
            17428, // Triclinic Bistot
            62565, // Compressed Triclinic Bistot
            28390, // Batch Compressed Triclinic Bistot
            17466, // Bright Spodumain
            62573, // Compressed Bright Spodumain
            28418, // Batch Compressed Bright Spodumain
            74530, // Doped Eifyrium
            75284, // Compressed Doped Eifyrium
            74534, // Noble Ducinium
            75288, // Compressed Noble Ducinium
            17426, // Prime Arkonor
            62570, // Compressed Prime Arkonor
            28387, // Batch Compressed Prime Arkonor
            17429, // Monoclinic Bistot
            62566, // Compressed Monoclinic Bistot
            28389, // Batch Compressed Monoclinic Bistot
            17467, // Gleaming Spodumain
            62574, // Compressed Gleaming Spodumain
            28419, // Batch Compressed Gleaming Spodumain
            74531, // Boosted Eifyrium
            75285, // Compressed Boosted Eifyrium
            74535, // Royal Ducinium
            75289, // Compressed Royal Ducinium
            46678, // Flawless Arkonor
            62571, // Compressed Flawless Arkonor
            46691, // Batch Compressed Flawless Arkonor
            46676, // Cubic Bistot
            62567, // Compressed Cubic Bistot
            46692, // Batch Compressed Cubic Bistot
            46688, // Dazzling Spodumain
            62575, // Compressed Dazzling Spodumain
            46704, // Batch Compressed Dazzling Spodumain
            74532, // Augmented Eifyrium
            75286, // Compressed Augmented Eifyrium
            74536, // Imperial Ducinium
            75290 // Compressed Imperial Ducinium
        ], _skills.ComplexOreProcessing);

        RegisterOre([
            1225, // Crokite
            62560, // Compressed Crokite
            28391, // Batch Compressed Crokite
            1232, // Dark Ochre
            62556, // Compressed Dark Ochre
            28394, // Batch Compressed Dark Ochre
            1229, // Gneiss
            62552, // Compressed Gneiss
            28397, // Batch Compressed Gneiss
            17432, // Sharp Crokite
            62561, // Compressed Sharp Crokite
            28393, // Batch Compressed Sharp Crokite
            17436, // Onyx Ochre
            62557, // Compressed Onyx Ochre
            28396, // Batch Compressed Onyx Ochre
            17865, // Iridescent Gneiss
            62553, // Compressed Iridescent Gneiss
            28398, // Batch Compressed Iridescent Gneiss
            17433, // Crystalline Crokite
            62562, // Compressed Crystalline Crokite
            28392, // Batch Compressed Crystalline Crokite
            17437, // Obsidian Ochre
            62558, // Compressed Obsidian Ochre
            28395, // Batch Compressed Obsidian Ochre
            17866, // Prismatic Gneiss
            62554, // Compressed Prismatic Gneiss
            28399, // Batch Compressed Prismatic Gneiss
            46677, // Pellucid Crokite
            62563, // Compressed Pellucid Crokite
            46693, // Batch Compressed Pellucid Crokite
            46679, // Pellucid Ochre
            62559, // Compressed Pellucid Ochre
            46694, // Batch Compressed Pellucid Ochre
            46679, // Brilliant Gneiss
            62555, // Compressed Brilliant Gneiss
            46695 // Batch Compressed Brilliant Gneiss
        ], _skills.VariegatedOreProcessing);

        RegisterOre([
            11396, // Mercoxit
            62586, // Compressed Mercoxit
            28413, // Batch Compressed Mercoxit
            17869, // Magma Mercoxit
            62587, // Compressed Magma Mercoxit
            28412, // Batch Compressed Magma Mercoxit
            17870, // Vitreous Mercoxit
            62588, // Compressed Vitreous Mercoxit
            28414 // Batch Compressed Vitreous Mercoxit
        ], _skills.MercoxitOreProcessing);

        RegisterOre([
            45494, // Cobaltite
            62474, // Compressed Cobaltite
            45495, // Euxenite
            62471, // Compressed Euxenite
            45496, // Titanite
            62477, // Compressed Titanite
            45497, // Scheelite
            62468, // Compressed Scheelite
            46288, // Copious Cobaltite
            62475, // Compressed Copious Cobaltite
            46290, // Copious Euxenite
            62472, // Compressed Copious Euxenite
            46292, // Copious Titanite
            62478, // Compressed Copious Titanite
            46294, // Copious Scheelite
            62469, // Compressed Copious Scheelite
            46289, // Twinkling Cobaltite
            62476, // Compressed Twinkling Cobaltite
            46291, // Twinkling Euxenite
            62473, // Compressed Twinkling Euxenite
            46293, // Twinkling Titanite
            62479, // Compressed Twinkling Titanite
            46295, // Twinkling Scheelite
            62470 // Compressed Twinkling Scheelite
        ], _skills.CommonMoonOreProcessing);

        RegisterOre([
            45510, // Xenotime
            62510, // Compressed Xenotime
            45511, // Monazite
            62507, // Compressed Monazite
            45512, // Loparite
            62504, // Compressed Loparite
            45513, // Ytterbite
            62513, // Compressed Ytterbite
            46312, // Bountiful Xenotime
            62511, // Compressed Bountiful Xenotime
            46314, // Bountiful Monazite
            62508, // Compressed Bountiful Monazite
            46316, // Bountiful Loparite
            62505, // Compressed Bountiful Loparite
            46318, // Bountiful Ytterbite
            62514, // Compressed Bountiful Ytterbite
            46313, // Shining Xenotime
            62512, // Compressed Shining Xenotime
            46315, // Shining Monazite
            62509, // Compressed Shining Monazite
            46317, // Shining Loparite
            62506, // Compressed Shining Loparite
            46319, // Shining Ytterbite
            62515 // Compressed Shining Ytterbite
        ], _skills.ExceptionalMoonOreProcessing);

        RegisterOre([
            45502, // Carnotite
            62492, // Compressed Carnotite
            45503, // Zircon
            62501, // Compressed Zircon
            45504, // Pollucite
            62498, // Compressed Pollucite
            45506, // Cinnabar
            62495, // Compressed Cinnabar
            46304, // Replete Carnotite
            62493, // Compressed Replete Carnotite
            46306, // Replete Zircon
            62502, // Compressed Replete Zircon
            46308, // Replete Pollucite
            62499, // Compressed Replete Pollucite
            46310, // Replete Cinnabar
            62496, // Compressed Replete Cinnabar
            46305, // Glowing Carnotite
            62494, // Compressed Glowing Carnotite
            46307, // Glowing Zircon
            62503, // Compressed Glowing Zircon
            46309, // Glowing Pollucite
            62500, // Compressed Glowing Pollucite
            46311, // Glowing Cinnabar
            62497 // Compressed Glowing Cinnabar
        ], _skills.RareMoonOreProcessing);

        RegisterOre([
            45490, // Zeolites
            62463, // Compressed Zeolites
            45491, // Sylvite
            62460, // Compressed Sylvite
            45492, // Bitumens
            62454, // Compressed Bitumens
            45493, // Coesite
            62457, // Compressed Coesite
            46280, // Brimful Zeolites
            62464, // Compressed Brimful Zeolites
            46282, // Brimful Sylvite
            62461, // Compressed Brimful Sylvite
            46284, // Brimful Bitumens
            62455, // Compressed Brimful Bitumens
            46286, // Brimful Coesite
            62458, // Compressed Brimful Coesite
            46281, // Glistening Zeolites
            62467, // Compressed Glistening Zeolites
            46283, // Glistening Sylvite
            62466, // Compressed Glistening Sylvite
            46285, // Glistening Bitumens
            62456, // Compressed Glistening Bitumens
            46287, // Glistening Coesite
            62459 // Compressed Glistening Coesite
        ], _skills.UbiquitousMoonOreProcessing);

        RegisterOre([
            45498, // Otavite
            62483, // Compressed Otavite
            45499, // Sperrylite
            62486, // Compressed Sperrylite
            45500, // Vanadinite
            62489, // Compressed Vanadinite
            45501, // Chromite
            62480, // Compressed Chromite
            46296, // Lavish Otavite
            62484, // Compressed Lavish Otavite
            46298, // Lavish Sperrylite
            62487, // Compressed Lavish Sperrylite
            46300, // Lavish Vanadinite
            62490, // Compressed Lavish Vanadinite
            46302, // Lavish Chromite
            62481, // Compressed Lavish Chromite
            46297, // Shimmering Otavite
            62485, // Compressed Shimmering Otavite
            46299, // Shimmering Sperrylite
            62488, // Compressed Shimmering Sperrylite
            46301, // Shimmering Vanadinite
            62491, // Compressed Shimmering Vanadinite
            46303, // Shimmering Chromite
            62482 // Compressed Shimmering Chromite
        ], _skills.UncommonMoonOreProcessing);

        RegisterOre([
            16266, // Glare Crust
            28439, // Compressed Glare Crust
            16267, // Dark Glitter
            28435, // Compressed Dark Glitter
            16268, // Gelidus
            28437, // Compressed Gelidus
            16269, // Krystallos
            28440, // Compressed Krystallos
            16262, // Clear Icicle
            28434, // Compressed Clear Icicle
            16265, // White Glaze
            28444, // Compressed White Glaze
            16264, // Blue Ice
            28433, // Compressed Blue Ice
            16263, // Glacial Mass
            28438 // Compressed Glacial Mass
        ], _skills.IceProcessing);
    }

    private void RegisterOre(List<OreId> oreIds, SkillId reprocessingSkillLevel)
    {
        foreach (var id in oreIds)
        {
            _oreReprocessingSkillLevels[id] = reprocessingSkillLevel;
        }
    }

    public SkillId GetOreReprocessingSkillLevel(OreId typeId)
    {
        return _oreReprocessingSkillLevels.GetValueOrDefault(typeId, 0);
    }
}