using EveStationJanitor.Core.DataAccess.Entities;

namespace EveStationJanitor.Core;

public record ItemFlipAppraisal(ItemType Item, SalesVolume QuantityToBuy, Isk CostOfGoodsSold, Isk Revenue, (int ItemId, SalesVolume Quantity)[] ReprocessedItems)
{
    public Isk GrossProfit => Revenue - CostOfGoodsSold;
    public decimal ProfitMargin => Revenue == 0 ? 0 : GrossProfit / Revenue;
}
