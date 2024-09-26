using EveStationJanitor.Authentication;
using EveStationJanitor.EveApi;
using OneOf.Types;

namespace EveStationJanitor.Core;

internal class EveCharacterData : IEveCharacterData
{
    private readonly IEveApi _eveApi;
    private readonly ITokenProvider _tokenProvider;

    public EveCharacterData(IEveApi esiClient, ITokenProvider tokenProvider)
    {
        _eveApi = esiClient;
        _tokenProvider = tokenProvider;
    }

    public async Task<ImplantsResult> GetImplants()
    {
        return await Task.FromResult(new Success());
    }

    public async Task<SkillsResult> GetSkills()
    {
        var token = await _tokenProvider.GetToken();
        if (token is null)
        {
            return new Error<string>("Could not acquire character information for skills request.");
        }

        // Try update the skills data
        var apiSkillsResult = await _eveApi.Character.GetCharacterSkills(token.CharacterId);
        return apiSkillsResult.Match<SkillsResult>(apiSkills =>
        {
            return new Skills(apiSkills);

        },
        error => error,
        notModified => new Error<string>("Not modified."));
    }

    public async Task<StandingsResult> GetStandings(Skills skills)
    {
        var token = await _tokenProvider.GetToken();
        if (token is null)
        {
            return new Error<string>("Could not acquire character information for skills request.");
        }

        var apiStandingsResult = await _eveApi.Character.GetCharacterStandings(token.CharacterId);
        return apiStandingsResult.Match<StandingsResult>(apiStandings =>
        {
            var standings = new Standings(skills);
            foreach (var standing in apiStandings)
            {
                standings.AddStanding(standing.FromId, standing.Standing);
            }
            return standings;
        },
        error => error,
        notModified => new Error<string>("Not modified."));
    }
}
