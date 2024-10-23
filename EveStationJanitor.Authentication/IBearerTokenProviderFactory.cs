namespace EveStationJanitor.Authentication;

public interface IBearerTokenProviderFactory
{
    IBearerTokenProvider Create(int characterId);
}
