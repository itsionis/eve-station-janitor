using EveStationJanitor.EveApi.Esi;

namespace EveStationJanitor.EveApi.Clone;

public interface ICloneApi
{
    Task<EveEsiResult<List<int>>> GetActiveCloneImplants(int characterId);
}
