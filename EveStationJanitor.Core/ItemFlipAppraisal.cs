using EveStationJanitor.Core.DataAccess.Entities;

namespace EveStationJanitor.Core;

public record ItemFlipAppraisal(ItemType Item, int QuantityToBuy, decimal CostOfGoodsSold, decimal Revenue,  (int ItemId, long Quantity)[] ReprocessedItems)
{
    public decimal GrossProfit => Revenue - CostOfGoodsSold;
    public decimal ProfitMargin => Revenue == 0 ? 0 : GrossProfit / Revenue;
}
