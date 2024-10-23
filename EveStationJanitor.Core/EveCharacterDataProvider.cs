using EveStationJanitor.EveApi;

namespace EveStationJanitor.Core;

public class EveCharacterDataProvider(IAuthenticatedEveApiProvider eveApiProvider) : IEveCharacterDataProvider
{
    public IEveCharacterData CreateForCharacter(int characterId)
    {
        return new EveCharacterData(eveApiProvider, characterId);
    }
}
