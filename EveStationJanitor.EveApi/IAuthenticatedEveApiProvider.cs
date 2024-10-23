using EveStationJanitor.EveApi.Character;
using EveStationJanitor.EveApi.Clone;

namespace EveStationJanitor.EveApi;

public interface IAuthenticatedEveApiProvider
{
    ICloneApi CreateCloneApi(int authCharacterId);
    ICharacterApi CreateCharacterApi(int authCharacterId);
}