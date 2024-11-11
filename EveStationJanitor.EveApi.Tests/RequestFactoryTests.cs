using EveStationJanitor.Authentication;
using EveStationJanitor.EveApi.Esi;
using Xunit;

namespace EveStationJanitor.EveApi.Tests;

public class RequestFactoryTests
{
    [Fact]
    public async Task CreateCharacterSkillsRequestTests()
    {
        const int characterId = 1000;
        const string url = "https://esi.evetech.net/latest/characters/1000/skills/?datasource=tranquility";
        
        var httpRequest = await CreateRequest((requestFactory, tokenProvider) =>
            requestFactory.CharacterSkillsRequest(characterId, tokenProvider));
        
        Assert.Equal(url, httpRequest.RequestUri?.ToString());
    }  
    
    [Fact]
    public async Task CreateCharacterStandingsRequestTests()
    {
        const int characterId = 1000;
        const string url = "https://esi.evetech.net/latest/characters/1000/standings/?datasource=tranquility";
        
        var httpRequest = await CreateRequest((requestFactory, tokenProvider) =>
            requestFactory.CharacterStandingsRequest(characterId, tokenProvider));
        
        Assert.Equal(url, httpRequest.RequestUri?.ToString());
    }
    
    [Fact]
    public async Task CreateCloneImplantsRequestTests()
    {
        const int characterId = 1000;
        const string url = "https://esi.evetech.net/latest/characters/1000/implants/?datasource=tranquility";
        
        var httpRequest = await CreateRequest((requestFactory, tokenProvider) =>
            requestFactory.CloneImplantsRequest(characterId, tokenProvider));
        
        Assert.Equal(url, httpRequest.RequestUri?.ToString());
    }

    private static async Task<HttpRequestMessage> CreateRequest<T>(Func<EveEsiRequestFactory, IBearerTokenProvider, EveEsiRequest<T>> configureRequest)
    {
        var tagProvider = new TestEntityTagProvider();
        var tokenProvider = new TestBearerTokenProvider();

        var requestFactory = new EveEsiRequestFactory(tagProvider);
        var request = configureRequest(requestFactory, tokenProvider);

        return await request.CreateHttpRequestMessage();
    }

    private class TestBearerTokenProvider :IBearerTokenProvider
    {
        public Task<string?> GetToken()
        {
            return Task.FromResult<string?>("TOKEN");
        }
    }
}
