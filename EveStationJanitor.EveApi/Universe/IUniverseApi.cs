using EveStationJanitor.EveApi.Universe.Objects;
using EveStationJanitor.EveApi.Esi;

namespace EveStationJanitor.EveApi.Universe;

public interface IUniverseApi
{
    Task<EveEsiResult<ApiItemType>> GetItemType(int id);

    Task<EveEsiResult<ApiItemGroup>> GetItemGroup(int id);

    Task<EveEsiResult<ApiItemCategory>> GetItemCategory(int id);
}
