using EveStationJanitor.Core.DataAccess;
using EveStationJanitor.EveApi;

namespace EveStationJanitor.Core;

public class EveCharacterDataProvider(AppDbContext context, IAuthenticatedEveApiProvider eveApiProvider) : IEveCharacterDataProvider
{
    public IEveCharacterData CreateForCharacter(int characterId)
    {
        return new EveCharacterData(context, eveApiProvider, characterId);
    }
}
