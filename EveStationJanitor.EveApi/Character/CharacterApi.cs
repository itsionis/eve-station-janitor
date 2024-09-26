using EveStationJanitor.EveApi.Character.Objects;

namespace EveStationJanitor.EveApi.Character;

internal sealed class CharacterApi(EveEsiClient client, EveEsiRequestFactory requestFactory) : ICharacterApi
{
    public async Task<EveEsiResult<ApiCharacterSkills>> GetCharacterSkills(int characterId)
    {
        var request = requestFactory.CharacterSkillsRequest(characterId);
        var result = await client.Get(request);
        return EveEsiResult.FromResult(result);
    }

    public async Task<EveEsiResult<List<ApiCharacterStanding>>> GetCharacterStandings(int characterId)
    {
        var request = requestFactory.CharacterStandingsRequest(characterId);
        var result = await client.Get(request);
        return EveEsiResult.FromResult(result);
    }
}

internal class CharacterSkillsEndpointSpec(int characterId) : IEveEsiEndpointSpec
{
    public HttpMethod HttpMethod => HttpMethod.Get;

    public string RelativeUrlPath => $"/characters/{characterId}/skills/";

    public object? QueryKeyValues => null;
}

internal class CharacterStandingsEndpointSpec(int characterId) : IEveEsiEndpointSpec
{
    public HttpMethod HttpMethod => HttpMethod.Get;

    public string RelativeUrlPath => $"/characters/{characterId}/standings/";

    public object? QueryKeyValues => null;
}
