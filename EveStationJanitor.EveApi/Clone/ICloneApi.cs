using EveStationJanitor.EveApi.Clone.Objects;
using EveStationJanitor.EveApi.Esi;

namespace EveStationJanitor.EveApi.Clone;

public interface ICloneApi
{
    Task<EveEsiResult<ApiCloneImplants>> GetCharacterImplants(int characterId);
}
