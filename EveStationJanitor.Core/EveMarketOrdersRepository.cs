using EveStationJanitor.Core.DataAccess;
using EveStationJanitor.Core.DataAccess.Entities;
using NodaTime.Text;
using NodaTime;
using OneOf;
using OneOf.Types;
using EveStationJanitor.EveApi.Market.Objects;
using EveStationJanitor.EveApi;
using Microsoft.EntityFrameworkCore;

namespace EveStationJanitor.Core;

internal class EveMarketOrdersRepository(AppDbContext context, IPublicEveApi eveApiProvider) : IEveMarketOrdersRepository
{
    public async Task<OneOf<Success, Error<string>>> LoadOrders(Station station)
    {
        Console.WriteLine($"Loading market orders in {station.SolarSystem.Region.Name}...");
        var apiOrders = await eveApiProvider.Markets.GetMarketOrders(station.SolarSystem.RegionId);

        if (apiOrders.TryPickT0(out var orders, out var otherwise))
        {
            Console.WriteLine("Saving market orders...");
            await SaveMarketOrders(station, orders);
            return new Success();
        }

        return otherwise.Match<OneOf<Success, Error<string>>>(
            error => error,
            notModified => new Success());
    }

    public async Task<List<AggregatedMarketOrder>> GetAggregateSellOrders(Station station)
    {
        var orders = await context.MarketOrders
            .AsNoTracking()
            .AsSplitQuery()
            .Include(order => order.ItemType)
            .ThenInclude(type => type.Materials)
            .ThenInclude(material => material.MaterialType)
            .Include(order => order.ItemType)
            .ThenInclude(itemType => itemType.Group)
            .ThenInclude(itemGroup => itemGroup.Category)
            .Where(order => !order.IsBuyOrder) // Sell orders
            .Where(order => order.LocationId == station.Id) // In our station
            .Where(order => order.ItemType.Materials.Any()) // Can be reprocessed
            .Select(order => new 
            {
                order.ItemType,
                order.Price,
                order.VolumeRemaining
            })
            .ToListAsync();

        return orders
            .GroupBy(order => new { order.ItemType.Id, order.Price })
            .Select(group => new AggregatedMarketOrder(
                group.First().ItemType,
                group.Key.Price,
                group.Sum(o => o.VolumeRemaining)))
            .OrderBy(order => order.ItemType.Id)
            .ThenBy(order => order.Price)
            .ToList();
    }

    public async Task<List<AggregatedMarketOrder>> GetAggregateBuyOrders(Station station)
    {
        var orders = await context.MarketOrders
            .AsNoTracking()
            .AsSplitQuery()
            .Include(order => order.ItemType)
            .Where(order => order.IsBuyOrder) // Buy orders
            .Where(order => order.LocationId == station.Id) // In our station
            .Select(order => new 
            {
                order.ItemType,
                order.Price,
                order.VolumeRemaining
            })
            .ToListAsync();

        return orders
            .GroupBy(order => new { order.ItemType.Id, order.Price })
            .Select(group => new AggregatedMarketOrder(
                group.First().ItemType,
                group.Key.Price,
                group.Sum(o => o.VolumeRemaining)))
            .OrderBy(order=>order.ItemType.Id)
            .ThenByDescending(order => order.Price)
            .ToList();
    }

    private async Task SaveMarketOrders(Station station, List<ApiMarketOrder> ordersFromApi)
    {
        var orders = ordersFromApi.Where(order => order.LocationId == station.Id)
            .Select(o => new MarketOrder
            {
                Duration = Duration.FromDays(o.Duration),
                TypeId = o.TypeId,
                IsBuyOrder = o.IsBuyOrder,
                Issued = InstantPattern.General.Parse(o.Issued).Value,
                LocationId = o.LocationId,
                MinVolume = o.MinVolume,
                OrderId = o.OrderId,
                Price = o.Price,
                Range = MarketOrder.ParseOrderRange(o.Range),
                SystemId = o.SystemId,
                VolumeTotal = o.VolumeTotal,
                VolumeRemaining = o.VolumeRemaining,
            }).ToList();

        // Sometimes the SDE is behind live data. Load any missing item, groups and categories to prevent foreign key violations
        await EnsureOrderItemsConsistency(orders);
        
        // Replace all existing
        var ordersToRemove = context.MarketOrders.Where(order => order.LocationId == station.Id);
        await ordersToRemove.ExecuteDeleteAsync();

        const int MarketOrderSaveBatchSize = 50_000;
        
        foreach (var batch in orders.Chunk(MarketOrderSaveBatchSize))
        {
            await context.MarketOrders.AddRangeAsync(batch);
            await context.SaveChangesAsync();
        }
    }

    private async Task<bool> EnsureOrderItemsConsistency(IReadOnlyList<MarketOrder> orders)
    {
        var itemIdsInOrders = orders.Select(order => order.TypeId).ToHashSet();
        var itemIdsInDatabase = context.ItemTypes.Select(item => item.Id).ToHashSet();
        var missingItemIds = itemIdsInOrders.Except(itemIdsInDatabase).ToList();

        if (missingItemIds.Count == 0)
        {
            return true;
        }

        foreach (var itemId in missingItemIds)
        {
            var isItem = await EnsureItem(itemId);
            if (!isItem)
            {
                return false;
            }
        }

        await context.SaveChangesAsync();
        return true;
    }

    private async Task<bool> EnsureItem(int itemId)
    {
        var maybeItem = context.ItemTypes.FirstOrDefault(itemType => itemType.Id == itemId);
        if (maybeItem is not null)
        {
            return true;
        }

        var apiItem = await eveApiProvider.Universe.GetItemType(itemId);
        if (!apiItem.TryPickT0(out var item, out var error))
        {
            Console.WriteLine($"Attempted to retrieve missing item {itemId}, but ESI did not return a result.");
            return false;
        }

        var isItemGroup = await EnsureItemGroup(item.GroupId);
        if (!isItemGroup)
        {
            return false;
        }

        var newItem = new ItemType
        {
            Id = item.TypeId,
            Name = item.Name,
            GroupId = item.GroupId,
            Mass = item.Mass,
            PortionSize = item.PortionSize,
            Volume = item.Volume
        };

        context.ItemTypes.Add(newItem);
        await context.SaveChangesAsync();

        return true;
    }

    private async Task<bool> EnsureItemGroup(int id)
    {
        var maybeItemGroup = context.ItemGroups.FirstOrDefault(group => group.Id == id);
        if (maybeItemGroup is not null)
        {
            return true;
        }

        var apiItemGroup = await eveApiProvider.Universe.GetItemGroup(id);
        if (!apiItemGroup.TryPickT0(out var itemGroup, out var error))
        {
            Console.WriteLine($"Tried to get missing item group with ID {id} but ESI did not return a result");
            return false;
        }

        var isItemCategory = await EnsureItemCategory(itemGroup.CategoryId);
        if (!isItemCategory)
        {
            return false;
        }

        var newItemGroup = new ItemGroup
        {
            Id = itemGroup.Id,
            Name = itemGroup.Name,
            CategoryId = itemGroup.CategoryId,
        };

        context.ItemGroups.Add(newItemGroup);
        await context.SaveChangesAsync();

        return true;
    }

    private async Task<bool> EnsureItemCategory(int id)
    {
        var maybeItemCategory = context.ItemCategories.FirstOrDefault(category => category.Id == id);
        if (maybeItemCategory is not null)
        {
            return true;
        }

        var apiItemCategory = await eveApiProvider.Universe.GetItemCategory(id);
        if (!apiItemCategory.TryPickT0(out var itemCategory, out var error))
        {
            Console.WriteLine($"Tried to get missing item category with ID {id} but ESI did not return a result");
            return false;
        }

        var newItemCategory = new ItemCategory
        {
            Id = itemCategory.Id,
            Name = itemCategory.Name,
        };

        context.ItemCategories.Add(newItemCategory);
        await context.SaveChangesAsync();

        return true;
    }
}