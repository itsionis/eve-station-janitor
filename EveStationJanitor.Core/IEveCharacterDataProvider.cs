namespace EveStationJanitor.Core;

public interface IEveCharacterDataProvider
{
    public IEveCharacterData CreateForCharacter(int characterId);
}
