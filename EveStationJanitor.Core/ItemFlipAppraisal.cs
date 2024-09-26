using EveStationJanitor.Core.DataAccess.Entities;

namespace EveStationJanitor.Core;

public record ItemFlipAppraisal(ItemType Item, int Volume, decimal CostOfGoodsSold, decimal Revenue)
{
    public decimal GrossProfit => Revenue - CostOfGoodsSold;
    public decimal ProfitMargin => GrossProfit / Revenue;
}
