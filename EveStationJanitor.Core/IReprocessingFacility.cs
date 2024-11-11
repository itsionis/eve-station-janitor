using EveStationJanitor.Core.DataAccess.Entities;

namespace EveStationJanitor.Core;

public interface IReprocessingFacility
{
    decimal ReprocessingTaxPercent { get; }
    long ReprocessedMaterialQuantity(ItemType itemBeingReprocessed, int baseReprocessedQuantity);
}
