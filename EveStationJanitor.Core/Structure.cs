using EveStationJanitor.Core.Eve;

namespace EveStationJanitor.Core;

public record Structure(string Name, StructureType Type, StructureReprocessingRigLevel ReprocessingRigLevel, SpaceSecurityType SecurityType, decimal ReprocessingTax);