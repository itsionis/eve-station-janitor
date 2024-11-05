using EveStationJanitor.Core.DataAccess.Entities;

namespace EveStationJanitor.Core;

public record ItemFlipAppraisal(ItemType Item, long QuantityToBuy, decimal CostOfGoodsSold, decimal Revenue,  (int ItemId, long Quantity)[] ReprocessedItems)
{
    private const decimal ProfitWeight = 0.75m;
    private const decimal MarginWeight = 0.25m;

    public decimal GrossProfit => Revenue - CostOfGoodsSold;
    public decimal ProfitMargin => Revenue == 0 ? 0 : GrossProfit / Revenue;
    public decimal Score
    {
        get
        {
            // Normalize profit on a scale from 0 to 1 using a logarithmic scale
            // This helps handle the large range of possible profit values
            var normalizedProfit = (decimal)Math.Log10((double)(GrossProfit + 1)) / 9m; // +1 to avoid log(0), divide by 9 as log10(1B) ≈ 9

            // Combine scores using weights
            return (normalizedProfit * ProfitWeight) + (ProfitMargin * MarginWeight);
        }
    }
}
