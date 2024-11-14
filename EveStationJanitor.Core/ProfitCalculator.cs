using EveStationJanitor.Core.DataAccess.Entities;
using EveStationJanitor.Core.Eve;
using EveStationJanitor.Core.Eve.Formula;

namespace EveStationJanitor.Core;

public class ProfitCalculator
{
    private readonly IEveMarketOrdersRepository _marketOrdersRepository;
    private readonly Station _tradeStation;
    private readonly IReprocessingFacility _reprocessingFacility;
    private readonly decimal _salesTransactionTaxPercent;

    public ProfitCalculator(IEveMarketOrdersRepository marketOrdersRepository, Station tradeStation, IReprocessingFacility reprocessingFacility, Skills skills)
    {
        _marketOrdersRepository = marketOrdersRepository;
        _tradeStation = tradeStation;
        _reprocessingFacility = reprocessingFacility;
        _salesTransactionTaxPercent = TaxFormula.SalesTransactionTax(skills.Accounting);
    }

    public async Task<List<ItemFlipAppraisal>> FindMostProfitableOrders(Isk minimumProfit = 0, int maxFlips = 200)
    {
        var sellOrders = await _marketOrdersRepository.GetAggregateSellOrders(_tradeStation);
        var buyOrders = await _marketOrdersRepository.GetAggregateBuyOrders(_tradeStation);
        
        var market = new SimulatedMarket(buyOrders);
        var flips = new List<ItemFlipAppraisal>();

        while (flips.Count < maxFlips)
        {
            var mostProfitable = FindMostProfitableOrder(market, sellOrders);
            if (mostProfitable is null)
            {
                return flips;
            }

            var (order, flip) = mostProfitable.Value;

            // The most profitable order is not profitable at all! Break out of the loop and present what we've found.
            if (flip.GrossProfit < minimumProfit)
            {
                break;
            }

            // Execute the reprocessed item sell orders on the market. This ensures our next most profitable item is
            // based on available buy orders.
            foreach (var (reprocessedItemToSell, quantity) in flip.ReprocessedItems)
            {
                market.ExecuteSell(reprocessedItemToSell, quantity);
            }
            
            sellOrders.Remove(order);
            flips.Add(flip);
        }

        return flips;
    }

    public (AggregatedMarketOrder Order, ItemFlipAppraisal Flip)? FindMostProfitableOrder(SimulatedMarket market, List<AggregatedMarketOrder> sellOrders)
    {
        (AggregatedMarketOrder, ItemFlipAppraisal)? best = null;
        var maxProfit = decimal.MinValue;

        var sellOrdersByItem = sellOrders.GroupBy(order => order.ItemType.Id);
        foreach (var itemSellOrders in sellOrdersByItem)
        {
            foreach (var sellOrder in itemSellOrders)
            {
                var flippableUnit = sellOrder.ItemType.PortionSize;
                var flippableVolume = (sellOrder.VolumeRemaining / flippableUnit) * flippableUnit;

                if (flippableVolume == 0)
                {
                    continue;
                }

                var appraisal = AppraiseFlip(market, sellOrder.ItemType, sellOrder.Price, flippableVolume);
                if (appraisal.GrossProfit > maxProfit)
                {
                    maxProfit = appraisal.GrossProfit;
                    best = (sellOrder, appraisal);
                }

                // If no further profits are possible for this item type, break early
                if (appraisal.GrossProfit <= 0)
                {
                    // break;
                }
            }
        }

        return best;
    }

    private ItemFlipAppraisal AppraiseFlip(SimulatedMarket market, ItemType itemToFlip, Isk price, SalesVolume quantity)
    {
        return AppraiseFlip(_salesTransactionTaxPercent, _reprocessingFacility, market, itemToFlip, price, quantity);
    }

    public static ItemFlipAppraisal AppraiseFlip(decimal salesTransactionTaxPercent, IReprocessingFacility reprocessing, SimulatedMarket market, ItemType itemToFlip, Isk price, SalesVolume quantity)
    {
        var revenue = 0m;
        
        // We may be flipping N items, but reprocessing works in "portion sizes". E.g. if you reprocess 1,000 missiles
        // which have a portion size of 500, then you're only reprocessing 2 times.
        var portions = itemToFlip.PortionSize == 0 ? 0 : quantity / itemToFlip.PortionSize;
        if (portions == 0)
        {
            return new ItemFlipAppraisal(itemToFlip, 0, 0, 0, []);
        }

        var reprocessedMaterials = new (int, long)[itemToFlip.Materials.Count];
        var materialIndex = 0;
        foreach (var material in itemToFlip.Materials)
        {
            // The item material quantity is the 100% ideal reprocessing yield. The actual yield is determined by the
            // station/structure reprocessing logic.
            var reprocessedMaterialsPerPortion = reprocessing.ReprocessedMaterialQuantity(itemBeingReprocessed: material.ItemType, material.Quantity);
            var materialQuantity = reprocessedMaterialsPerPortion * portions;
            
            // Simulate selling the reprocessed materials
            var materialRevenue = market.PreviewSell(material.MaterialItemTypeId, materialQuantity);

            {
                // Deduct the station's tax (equipment tax)
                materialRevenue *= (1 - reprocessing.ReprocessingTaxPercent);
                
                // Deduct the sales transaction tax (applies when selling an item back to the market)
                materialRevenue *= (1 - salesTransactionTaxPercent);
            }
            
            // Accumulate the material value across all materials reprocessed
            revenue += materialRevenue;
            
            reprocessedMaterials[materialIndex++] = (material.MaterialItemTypeId, materialQuantity);
        }

        var costOfGoodsSold = (decimal)price * quantity;
        return new ItemFlipAppraisal(itemToFlip, quantity, costOfGoodsSold, revenue, reprocessedMaterials);
    }
}
