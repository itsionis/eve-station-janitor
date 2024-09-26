using EveStationJanitor.Authentication.Tokens;
using EveStationJanitor.EveApi.Character.Objects;

namespace EveStationJanitor.EveApi.Character;

public interface ICharacterApi
{
    Task<EveEsiResult<ApiCharacterSkills>> GetCharacterSkills(int characterId);
    Task<EveEsiResult<List<ApiCharacterStanding>>> GetCharacterStandings(int characterId);
}