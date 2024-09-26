using OneOf;
using OneOf.Types;

namespace EveStationJanitor.Core;

public interface IEveCharacterData
{
    public Task<SkillsResult> GetSkills();

    public Task<StandingsResult> GetStandings(Skills skills);

    public Task<ImplantsResult> GetImplants();
}

[GenerateOneOf]
public partial class SkillsResult : OneOfBase<Skills, Error<string>>;

[GenerateOneOf]
public partial class StandingsResult : OneOfBase<Standings, Error<string>>;

[GenerateOneOf]
public partial class ImplantsResult : OneOfBase<Success, Error<string>>;
