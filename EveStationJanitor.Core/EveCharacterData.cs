using EveStationJanitor.Core.DataAccess;
using EveStationJanitor.EveApi;
using OneOf.Types;

namespace EveStationJanitor.Core;

internal class EveCharacterData(AppDbContext context, IAuthenticatedEveApiProvider eveApiProvider, int characterId) : IEveCharacterData
{
    public async Task<ImplantsResult> GetActiveCloneImplants()
    {
        var cloneApi = eveApiProvider.CreateCloneApi(characterId);
        var apiImplantsResult = await cloneApi.GetActiveCloneImplants(characterId);
        return apiImplantsResult.Match<ImplantsResult>(implantIds =>
        {
            var implants = new CloneImplants();
            var implantItems = context.ItemTypes.Where(item => implantIds.Contains(item.Id)).ToList();
            implants.AddImplants(implantItems);
            return implants;
        },
        error => error,
        notModified => new Error<string>("Not modified."));
    }

    public async Task<SkillsResult> GetSkills()
    {
        var characterApi = eveApiProvider.CreateCharacterApi(characterId);
        var apiSkillsResult = await characterApi.GetCharacterSkills(characterId);
        return apiSkillsResult.Match<SkillsResult>(apiSkills =>
        {
            return new Skills(apiSkills);

        },
        error => error,
        notModified => new Error<string>("Not modified."));
    }

    public async Task<StandingsResult> GetStandings(Skills skills)
    {
        var characterApi = eveApiProvider.CreateCharacterApi(characterId);
        var apiStandingsResult = await characterApi.GetCharacterStandings(characterId);
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
