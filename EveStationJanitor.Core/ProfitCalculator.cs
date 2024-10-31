using EveStationJanitor.Core.DataAccess;
using EveStationJanitor.Core.DataAccess.Entities;
using EveStationJanitor.Core.Eve;
using EveStationJanitor.Core.Eve.Formula;
using Microsoft.EntityFrameworkCore;

namespace EveStationJanitor.Core;

public class ProfitCalculator
{
    private readonly AppDbContext _context;
    private readonly StationReprocessing _stationReprocessing;
    private readonly decimal _salesTransactionTaxPercent;

    public ProfitCalculator(AppDbContext context, StationReprocessing stationReprocessing, Skills skills)
    {
        _context = context;
        _stationReprocessing = stationReprocessing;
        _salesTransactionTaxPercent = TaxFormula.SalesTransactionTax(skills.Accounting);
    }

    public async Task<List<ItemFlipAppraisal>> FindMostProfitableOrders()
    {
        var sellOrders = await _context.MarketOrders
            .AsNoTracking()
            .AsSplitQuery()
            .Include(order => order.ItemType)
            .ThenInclude(type => type.Materials)
            .ThenInclude(material => material.MaterialType)
            .Include(order => order.ItemType)
            .ThenInclude(itemType => itemType.Group)
            .ThenInclude(itemGroup => itemGroup.Category)
            .Where(order => order.IsBuyOrder == false) // Sell orders
            .Where(order => order.Station == _stationReprocessing.Station) // In our station
            .Where(order => order.ItemType.Materials.Any()) // Can be reprocessed
            .ToListAsync();

        var buyOrders = await _context.MarketOrders
            .AsNoTracking()
            .AsSplitQuery()
            .Where(order => order.IsBuyOrder == true) // Buy orders
            .Where(order => order.Station == _stationReprocessing.Station) // In our station
            .Include(order => order.ItemType)
            .ToListAsync();

        var market = new SimulatedMarket(buyOrders);
        var flips = new List<ItemFlipAppraisal>();

        while (flips.Count < 250)
        {
            var mostProfitable = FindMostProfitableOrder(market, sellOrders);
            if (mostProfitable is null)
            {
                return flips;
            }

            var (order, flip) = mostProfitable.Value;

            // The most profitable order is not profitable at all! Break out of the loop and present what we've found.
            if (flip.GrossProfit < 0)
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

    public (MarketOrder Order, ItemFlipAppraisal Flip)? FindMostProfitableOrder(SimulatedMarket market, List<MarketOrder> sellOrders)
    {
        var sellOrdersByItemId = sellOrders
            .GroupBy(order => order.TypeId)
            .ToDictionary(
            group => group.Key,
            group => group.OrderBy(order => order.Price).ToList());

        (MarketOrder, ItemFlipAppraisal)? best = null;
        var maxProfit = decimal.MinValue;

        foreach (var orderGroup in sellOrdersByItemId)
        {
            foreach (var sellOrder in orderGroup.Value)
            {
                var flippableUnit = sellOrder.ItemType.PortionSize;
                var flippableVolume = (sellOrder.VolumeRemaining / flippableUnit) * flippableUnit;

                // TODO - Could combine with other orders to acquire a flippable quantity
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
                    break;
                }
            }
        }

        return best;
    }

    private ItemFlipAppraisal AppraiseFlip(SimulatedMarket market, ItemType itemToFlip, double price, int quantity)
    {
        return AppraiseFlip(_salesTransactionTaxPercent, _stationReprocessing, market, itemToFlip, price, quantity);
    }

    public static ItemFlipAppraisal AppraiseFlip(decimal salesTransactionTaxPercent, StationReprocessing reprocessing, SimulatedMarket market, ItemType itemToFlip, double price, int quantity)
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
            var reprocessedMaterialsPerPortion = reprocessing.ReprocessedMaterialQuantity(material);
            var materialQuantity = reprocessedMaterialsPerPortion * portions;
            
            // Simulate selling the reprocessed materials
            var materialRevenue = market.PreviewSell(material.MaterialItemTypeId, materialQuantity);

            {
                // Deduct the station's tax (equipment tax)
                materialRevenue *= (1 - reprocessing.StationReprocessingTaxPercent);
                
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
