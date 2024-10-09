using EveStationJanitor.EveApi.Universe.Objects;

namespace EveStationJanitor.EveApi.Universe;

public interface IUniverseApi
{
    Task<EveEsiResult<ApiItemType>> GetItemType(int id);
}
