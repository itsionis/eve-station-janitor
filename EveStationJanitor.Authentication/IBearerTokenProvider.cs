namespace EveStationJanitor.Authentication;

public interface IBearerTokenProvider
{
    Task<string?> GetToken();
}
