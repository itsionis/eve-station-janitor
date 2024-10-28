using Xunit;

namespace EveStationJanitor.Core.Tests;

public class SimulatedMarketTests
{
    [Fact]
    public void PreviewSellToSingleOrder_ReturnsBasicRevenue()
    {
        var market = new SimulatedMarket();
        market.AddOrder(1, 2, 100);

        Assert.Equal(100, market.PreviewSell(1, 50));
        Assert.Equal(200, market.PreviewSell(1, 100));
        Assert.Equal(200, market.PreviewSell(1, 150));
    }

    [Fact]
    public void PreviewSellToEmptyMarket_ReturnsZero()
    {
        var market = new SimulatedMarket();
        Assert.Equal(0, market.PreviewSell(1, 50));
    }

    [Fact]
    public void PreviewSellToMultipleOrders_ReturnsRevenue()
    {
        var market = new SimulatedMarket();
        market.AddOrder(1, 2, 100);
        market.AddOrder(1, 1, 100);
        
        Assert.Equal(100, market.PreviewSell(1, 50));
        Assert.Equal(200, market.PreviewSell(1, 100));
        Assert.Equal(250, market.PreviewSell(1, 150));
        Assert.Equal(300, market.PreviewSell(1, 200));
        Assert.Equal(300, market.PreviewSell(1, 250));
    }

    [Fact]
    public void ExecuteSellToSingleOrder_ExhaustsVolume()
    {
        var market = new SimulatedMarket();
        market.AddOrder(1, 2, 100);

        Assert.Equal(100, market.ExecuteSell(1, 50));
        Assert.Equal(100, market.ExecuteSell(1, 50));
        Assert.Equal(0, market.ExecuteSell(1, 50));
    }

    [Fact]
    public void ExecuteSellToMultipleOrders_ExhaustsVolume()
    {
        var market = new SimulatedMarket();
        market.AddOrder(1, 2, 100);
        market.AddOrder(1, 1, 100);
        
        Assert.Equal(250, market.ExecuteSell(1, 150));
        Assert.Equal(50, market.ExecuteSell(1, 150));
        Assert.Equal(0, market.ExecuteSell(1, 1));
    }
}
