using EveStationJanitor.Authentication.Tokens;
using EveStationJanitor.EveApi.Clone.Objects;

namespace EveStationJanitor.EveApi.Clone;

public interface ICloneApi
{
    Task<EveEsiResult<ApiCloneImplants>> GetCharacterImplants(int characterId);
}
