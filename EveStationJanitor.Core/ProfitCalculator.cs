using EveStationJanitor.Core.DataAccess;
using EveStationJanitor.Core.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace EveStationJanitor.Core;

public class ProfitCalculator
{
    private const decimal _baseSalesTaxPercent = 0.045m;

    private readonly AppDbContext _context;
    private readonly StationReprocessing _stationReprocessing;
    private readonly decimal _salesTransactionTaxPercent;

    public ProfitCalculator(AppDbContext context, StationReprocessing stationReprocessing, Skills skills)
    {
        _context = context;
        _stationReprocessing = stationReprocessing;
        _salesTransactionTaxPercent = _baseSalesTaxPercent * (1 - (0.11m * skills.Accounting));
    }

    public async Task<List<ItemFlipAppraisal>> FindMostProfitableOrders()
    {
        var sellOrders = await _context.MarketOrders
            .AsNoTracking()
            .Include(order => order.ItemType)
            .ThenInclude(type => type.Materials)
            .ThenInclude(material => material.MaterialType)
            .Include(order => order.ItemType)
            .ThenInclude(itemType => itemType.Group)
            .ThenInclude(itemGroup => itemGroup.Category)
            .Where(order => order.IsBuyOrder == false) // Sell orders
            .Where(order => order.Station == _stationReprocessing.Station) // In our station
            .Where(order => order.Price >= 10_000) // Remove out tiny items
            .Where(order => order.ItemType.Materials.Any()) // Can be reprocessed
            .ToListAsync();

        var buyOrders = await _context.MarketOrders
            .AsNoTracking()
            .Where(order => order.IsBuyOrder == true) // Buy orders
            .Where(order => order.Station == _stationReprocessing.Station) // In our station
            .Include(order => order.ItemType)
            .ToListAsync();

        var buyOrder95thPrices = CalculateOrderPercentilePricesPerItem(buyOrders, percentile: 0.95);
        var flips = new List<ItemFlipAppraisal>();

        while (true)
        {
            var mostProfitable = FindMostProfitableOrder(sellOrders, buyOrder95thPrices);
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

            sellOrders.Remove(order);
            flips.Add(flip);
        }

        return flips;
    }

    private static Dictionary<int, decimal> CalculateOrderPercentilePricesPerItem(List<MarketOrder> buyOrders, double percentile)
    {
        var result = new Dictionary<int, decimal>();

        foreach (var orders in buyOrders.GroupBy(o => o.TypeId))
        {
            var count = orders.Count();
            if (count == 0)
            {
                continue;
            }

            var index = (int)Math.Ceiling(percentile * count) - 1;

            var sortedOrders = orders.OrderBy(order => order.Price).ToList();
            result[orders.Key] = (decimal)sortedOrders[index].Price;
        }

        return result;
    }

    public (MarketOrder Order, ItemFlipAppraisal Flip)? FindMostProfitableOrder(List<MarketOrder> sellOrders, Dictionary<int, decimal> itemPercentileBuyPrices)
    {
        var groupedSellOrders = sellOrders
            .GroupBy(order => order.TypeId)
            .ToDictionary(
            group => group.Key,
            group => group.OrderBy(order => order.Price).ToList());

        (MarketOrder, ItemFlipAppraisal)? best = null;
        decimal maxProfit = decimal.MinValue;

        foreach (var orderGroup in groupedSellOrders)
        {
            foreach (var sellOrder in orderGroup.Value)
            {
                if (sellOrder.VolumeRemaining < sellOrder.ItemType.PortionSize)
                {
                    // Can't flip this order!
                    continue;
                }

                var appraisal = AppraiseFlip(sellOrder, itemPercentileBuyPrices);

                // Update if this sell order has a higher profit than the current best one
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

        return best;  // Return the most profitable order
    }

    public ItemFlipAppraisal AppraiseFlip(MarketOrder sellOrder, IReadOnlyDictionary<int, decimal> itemPercentileBuyPrices)
    {
        var item = sellOrder.ItemType;
        var totalMaterialValue = 0m;

        foreach (var material in item.Materials)
        {
            if (!itemPercentileBuyPrices.TryGetValue(material.MaterialItemTypeId, out var materialPrice))
            {
                // Material cannot be sold at station, continue just in case the rest of the materials can produce a profit anyway
                continue;
            }

            // Deduct the station's tax (equipment tax)
            materialPrice *= (1 - _stationReprocessing.StationReprocessingTaxPercent);

            // Deduct the sales transaction tax (applies when selling an item back to the market)
            materialPrice *= (1 - _salesTransactionTaxPercent);

            // The material quantity is the 100% ideal reprocessing yield. This is modified by the yield efficiency at the station
            var yield = Math.Truncate(material.Quantity * _stationReprocessing.CalculateYieldEfficiency(item));

            // Accumulate the running material total
            totalMaterialValue += materialPrice * yield;
        }

        // Calculate profit: value of materials - cost of the original item
        var costOfGoodsSold = (decimal)(sellOrder.Price * sellOrder.VolumeRemaining);
        return new ItemFlipAppraisal(item, sellOrder.VolumeRemaining, costOfGoodsSold, totalMaterialValue);
    }
}
