﻿using EveStationJanitor.Core.DataAccess.Entities;

namespace EveStationJanitor.Core;

public class SimulatedMarket
{
    private readonly Dictionary<int, PriorityQueue<BuyOrder, decimal>> _buyOrders = new();

    public SimulatedMarket()
    {
    }
    
    public SimulatedMarket(IReadOnlyCollection<MarketOrder> buyOrders)
    {
        foreach (var order in buyOrders)
        {
            AddOrder(order.TypeId, order.Price, order.VolumeRemaining);
        }
    }

    public void AddOrder(int itemTypeId, double price, int volumeRemaining)
    {
        if (!_buyOrders.TryGetValue(itemTypeId, out var itemBuyOrders))
        {
            itemBuyOrders = new PriorityQueue<BuyOrder, decimal>();
            _buyOrders[itemTypeId] = itemBuyOrders;
        }

        var buyOrder = new BuyOrder 
        { 
            Price = (decimal)price,
            VolumeRemaining = volumeRemaining
        };
            
        // Note the negative price. Priority queue pops item with the lowest value first. So higher buy-orders are
        // fulfilled first, meaning we need to negate the price.
        itemBuyOrders.Enqueue(buyOrder, -buyOrder.Price);
    }

    public decimal PreviewSell(int typeId, long quantity)
    {
        if (!_buyOrders.TryGetValue(typeId, out var orderQueue) || orderQueue.Count == 0)
        {
            return 0m;
        }

        var remainingQuantity = quantity;
        var totalSaleRevenue = 0m;
        
        // To preview we actively pop from the PriorityQueue and restore them at the end of the method. This Stack
        // keeps track of the orders.
        var poppedOrders = new Stack<BuyOrder>();

        try
        {
            while (remainingQuantity > 0 && orderQueue.Count > 0)
            {
                var order = orderQueue.Dequeue();
                poppedOrders.Push(order);
                
                var fillAmount = Math.Min(remainingQuantity, order.VolumeRemaining);
                totalSaleRevenue += order.Price * fillAmount;
                remainingQuantity -= fillAmount;
            }

            return totalSaleRevenue;
        }
        finally
        {
            // Restore the queue to its original state
            while (poppedOrders.Count > 0)
            {
                var order = poppedOrders.Pop();
                orderQueue.Enqueue(order, -order.Price);
            }
        }
    }

    public decimal ExecuteSell(int typeId, long quantity)
    {
        if (!_buyOrders.TryGetValue(typeId, out var orderQueue))
        {
            return 0m;
        }

        var remainingQuantity = quantity;
        var totalRevenue = 0m;

        while (remainingQuantity > 0 && orderQueue.Count > 0)
        {
            var order = orderQueue.Dequeue();
            var fillAmount = Math.Min(remainingQuantity, order.VolumeRemaining);
            
            totalRevenue += order.Price * fillAmount;

            if (order.VolumeRemaining > fillAmount)
            {
                // Partially filled order goes back to queue with reduced volume
                orderQueue.Enqueue(order.WithVolume(order.VolumeRemaining - (int)fillAmount), -order.Price);
            }
            // Completely filled orders are discarded
            
            remainingQuantity -= fillAmount;
        }

        if (orderQueue.Count == 0)
        {
            _buyOrders.Remove(typeId);
        }

        return totalRevenue;
    }
    
    public readonly record struct BuyOrder
    {
        public required decimal Price { get; init; }
        public required int VolumeRemaining { get; init; }

        public BuyOrder WithVolume(int newVolume) => this with { VolumeRemaining = newVolume };
    }
}