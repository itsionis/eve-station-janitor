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

        var orders = await _eveApi.Markets.GetMarketOrders(station.SolarSystem.RegionId, null, ApiMarketOrderType.All);

        var isError = false;

        orders.Switch(
            ordersFromApi => SaveMarketOrders(station, ordersFromApi),
            error => { isError = true; Console.WriteLine(error.Value); },
            notModified => { });

        if (isError) return new Error();

        return new Success();
    }

    private void SaveMarketOrders(Station station, List<ApiMarketOrder> ordersFromApi)
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
            });

        // Replace all existing
        var ordersToRemove = _context.MarketOrders.Where(order => order.LocationId == station.Id);
        _context.MarketOrders.RemoveRange(ordersToRemove);

        _context.MarketOrders.AddRange(orders);
        _context.SaveChanges();
    }
}
