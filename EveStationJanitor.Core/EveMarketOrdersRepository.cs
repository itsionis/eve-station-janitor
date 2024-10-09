using EveStationJanitor.Core.DataAccess;
using EveStationJanitor.Core.DataAccess.Entities;
using NodaTime.Text;
using NodaTime;
using OneOf;
using OneOf.Types;
using EveStationJanitor.EveApi;
using EveStationJanitor.EveApi.Market.Objects;

namespace EveStationJanitor.Core;

public class EveMarketOrdersRepository : IEveMarketOrdersRepository
{
    private readonly AppDbContext _context;
    private readonly IEveApi _eveApi;

    public EveMarketOrdersRepository(AppDbContext context, IEveApi eveApi)
    {
        _context = context;
        _eveApi = eveApi;
    }

    public async Task<OneOf<Success, Error>> LoadOrders(Station station)
    {
        Console.WriteLine($"Loading market orders in {station.SolarSystem.Region.Name}...");

        var apiOrders = await _eveApi.Markets.GetMarketOrders(station.SolarSystem.RegionId, null, ApiMarketOrderType.All);

        if(apiOrders.TryPickT0(out var orders, out var error))
        {
            await SaveMarketOrders(station, orders);
            return new Success();
        }
        else
        {
            return new Error();
        }
    }

    private async Task SaveMarketOrders(Station station, List<ApiMarketOrder> ordersFromApi)
    {
        var orders = ordersFromApi.Where(order => order.LocationId == station.Id)
            .Select(o =>
            {
                return new MarketOrder
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
                };
            }).ToList();

        // Sometimes the SDE is behind live data. Load any missing item, groups and categories to prevent foreign key violations
        await EnsureOrderItemsConsistency(orders);

        // Replace all existing
        var ordersToRemove = _context.MarketOrders.Where(order => order.LocationId == station.Id);
        _context.MarketOrders.RemoveRange(ordersToRemove);
        _context.MarketOrders.AddRange(orders);
        await _context.SaveChangesAsync();
    }

    private async Task<bool> EnsureOrderItemsConsistency(IReadOnlyList<MarketOrder> orders)
    {
        var itemIdsInOrders = orders.Select(order => order.TypeId).ToHashSet();
        var itemIdsInDatabase = _context.ItemTypes.Select(item => item.Id).ToHashSet();
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

        await _context.SaveChangesAsync();
        return true;
    }

    private async Task<bool> EnsureItem(int itemId)
    {
        var maybeItem = _context.ItemTypes.FirstOrDefault(item => item.Id == itemId);
        if (maybeItem is not null)
        {
            return true;
        }

        var apiItem = await _eveApi.Universe.GetItemType(itemId);
        if (!apiItem.TryPickT0(out var item, out var error))
        {
            Console.WriteLine($"Attempted to retrieve missing item {itemId}, but ESI did not return a result.");
            return false;
        }

        var isItemGroup = await EnsureItemGroup(item.GroupId);

        var newItem = new ItemType
        {
            Id = item.TypeId,
            Name = item.Name,
            GroupId = item.GroupId,
            Mass = item.Mass,
            PortionSize = item.PortionSize,
            Volume = item.Volume
        };

        _context.ItemTypes.Add(newItem);
        _context.SaveChanges();

        return true;
    }

    private async Task<bool> EnsureItemGroup(int id)
    {
        var maybeItemGroup = _context.ItemGroups.FirstOrDefault(group => group.Id == id);
        if (maybeItemGroup is not null)
        {
            return true;
        }

        var apiItemGroup = await _eveApi.Universe.GetItemGroup(id);
        if (!apiItemGroup.TryPickT0(out var itemGroup, out var error))
        {
            Console.WriteLine($"Tried to get missing item group with ID {id} but ESI did not return a result");
            return false;
        }

        await EnsureItemCategory(itemGroup.CategoryId);

        var newItemGroup = new ItemGroup
        {
            Id = itemGroup.Id,
            Name = itemGroup.Name,
            CategoryId = itemGroup.CategoryId,
        };

        _context.ItemGroups.Add(newItemGroup);
        _context.SaveChanges();

        return true;
    }

    private async Task<bool> EnsureItemCategory(int id)
    {
        var maybeItemCategory = _context.ItemCategories.FirstOrDefault(category => category.Id == id);
        if (maybeItemCategory is not null)
        {
            return true;
        }

        var apiItemCategory = await _eveApi.Universe.GetItemCategory(id);
        if (!apiItemCategory.TryPickT0(out var itemCategory, out var error))
        {
            Console.WriteLine($"Tried to get missing item categoory with ID {id} but ESI did not return a result");
            return false;
        }

        var newItemCategory = new ItemCategory
        {
            Id = itemCategory.Id,
            Name = itemCategory.Name,
        };

        _context.ItemCategories.Add(newItemCategory);
        _context.SaveChanges();

        return true;
    }
}
