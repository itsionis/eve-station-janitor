using EveStationJanitor.Core.DataAccess.Entities;

namespace EveStationJanitor.Core;

public record AggregatedMarketOrder(ItemType ItemType, Isk Price, SalesVolume VolumeRemaining);
