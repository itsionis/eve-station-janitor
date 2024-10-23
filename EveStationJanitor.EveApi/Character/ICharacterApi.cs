using EveStationJanitor.EveApi.Character.Objects;
using EveStationJanitor.EveApi.Esi;

namespace EveStationJanitor.EveApi.Character;

public interface ICharacterApi
{
    Task<EveEsiResult<ApiCharacterSkills>> GetCharacterSkills(int characterId);
    Task<EveEsiResult<List<ApiCharacterStanding>>> GetCharacterStandings(int characterId);
}