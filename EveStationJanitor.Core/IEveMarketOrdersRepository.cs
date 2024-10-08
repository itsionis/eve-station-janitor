﻿using EveStationJanitor.Core.DataAccess.Entities;
using OneOf.Types;
using OneOf;

namespace EveStationJanitor.Core;

public interface IEveMarketOrdersRepository
{
    Task<OneOf<Success, Error>> LoadOrders(Station station);
}
