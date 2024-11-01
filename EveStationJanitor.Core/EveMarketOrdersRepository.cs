﻿using EveStationJanitor.Core.DataAccess;
using EveStationJanitor.Core.DataAccess.Entities;
using NodaTime.Text;
using NodaTime;
using OneOf;
using OneOf.Types;
using EveStationJanitor.EveApi.Market.Objects;
using EveStationJanitor.EveApi;
using Microsoft.EntityFrameworkCore;

namespace EveStationJanitor.Core;

internal class EveMarketOrdersRepository : IEveMarketOrdersRepository
{
    private readonly AppDbContext _context;
    private readonly IPublicEveApi _eveApiProvider;

    public EveMarketOrdersRepository(AppDbContext context, IPublicEveApi eveApiProvider)
    {
        _context = context;
        _eveApiProvider = eveApiProvider;
    }

    public async Task<OneOf<Success, Error<string>>> LoadOrders(Station station)
    {
        Console.WriteLine($"Loading market orders in {station.SolarSystem.Region.Name}...");
        var apiOrders = await _eveApiProvider.Markets.GetMarketOrders(station.SolarSystem.RegionId);

        if (apiOrders.TryPickT0(out var orders, out var otherwise))
        {
            Console.WriteLine("Saving market orders...");
            await SaveMarketOrders(station, orders);
            return new Success();
        }

        return otherwise.Match<OneOf<Success, Error<string>>>(
            error => error,
            notModified =>
            {
                // Sales data is not modified, what we have saved is up-to-date. Continue.
                return new Success();
            });
    }

    public async Task<List<MarketOrder>> GetSellOrders(Station station)
    {
        return await _context.MarketOrders
            .AsNoTracking()
            .AsSplitQuery()
            .Include(order => order.ItemType)
            .ThenInclude(type => type.Materials)
            .ThenInclude(material => material.MaterialType)
            .Include(order => order.ItemType)
            .ThenInclude(itemType => itemType.Group)
            .ThenInclude(itemGroup => itemGroup.Category)
            .Where(order => order.IsBuyOrder == false) // Sell orders
            .Where(order => order.Station == station) // In our station
            .Where(order => order.ItemType.Materials.Any()) // Can be reprocessed
            .ToListAsync();
    }

    public async Task<List<MarketOrder>> GetBuyOrders(Station station)
    {
        return await _context.MarketOrders
            .AsNoTracking()
            .AsSplitQuery()
            .Where(order => order.IsBuyOrder == true) // Buy orders
            .Where(order => order.Station == station) // In our station
            .Include(order => order.ItemType)
            .ToListAsync();
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
        var maybeItem = _context.ItemTypes.FirstOrDefault(itemType => itemType.Id == itemId);
        if (maybeItem is not null)
        {
            return true;
        }

        var apiItem = await _eveApiProvider.Universe.GetItemType(itemId);
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

        var apiItemGroup = await _eveApiProvider.Universe.GetItemGroup(id);
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

        var apiItemCategory = await _eveApiProvider.Universe.GetItemCategory(id);
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
