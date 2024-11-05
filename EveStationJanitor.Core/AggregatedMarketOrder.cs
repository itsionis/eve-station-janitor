using EveStationJanitor.Core.DataAccess.Entities;

namespace EveStationJanitor.Core;

public record AggregatedMarketOrder(ItemType ItemType, double Price, long VolumeRemaining);
