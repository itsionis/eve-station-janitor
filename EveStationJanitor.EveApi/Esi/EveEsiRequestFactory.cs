using EveStationJanitor.Authentication;
using EveStationJanitor.EveApi.Character;
using EveStationJanitor.EveApi.Character.Objects;
using EveStationJanitor.EveApi.Clone;
using EveStationJanitor.EveApi.Market;
using EveStationJanitor.EveApi.Market.Objects;
using EveStationJanitor.EveApi.Status;
using EveStationJanitor.EveApi.Status.Objects;
using EveStationJanitor.EveApi.Universe;
using EveStationJanitor.EveApi.Universe.Objects;

namespace EveStationJanitor.EveApi.Esi;

internal class EveEsiRequestFactory(IEntityTagProvider entityTagProvider)
{
    public EveEsiRequest<List<int>> CloneImplantsRequest(int characterId, IBearerTokenProvider tokenProvider)
    {
        var spec = new CloneImplantsEndpointSpec(characterId);
        return new EveEsiRequest<List<int>>(spec, entityTagProvider, tokenProvider);
    }

    public EveEsiRequest<ApiCharacterSkills> CharacterSkillsRequest(int characterId, IBearerTokenProvider tokenProvider)
    {
        var spec = new CharacterSkillsEndpointSpec(characterId);
        return new EveEsiRequest<ApiCharacterSkills>(spec, entityTagProvider, tokenProvider);
    }

    public EveEsiRequest<List<ApiCharacterStanding>> CharacterStandingsRequest(int characterId, IBearerTokenProvider tokenProvider)
    {
        var spec = new CharacterStandingsEndpointSpec(characterId);
        return new EveEsiRequest<List<ApiCharacterStanding>>(spec, entityTagProvider, tokenProvider);
    }

    public EveEsiPagedRequest<ApiMarketOrder> MarketOrdersRequest(int regionId, int? itemTypeId, ApiMarketOrderType orderType)
    {
        var spec = new MarketOrdersEndpointSpec(regionId, itemTypeId, orderType);
        return new EveEsiPagedRequest<ApiMarketOrder>(spec, entityTagProvider);
    }

    public EveEsiRequest<ApiItemType> ItemTypeRequest(int itemId)
    {
        var spec = new ItemTypeEndpointSpec(itemId);
        return new EveEsiRequest<ApiItemType>(spec, entityTagProvider);
    }

    public EveEsiRequest<ApiItemGroup> ItemGroupRequest(int itemGroupId)
    {
        var spec = new ItemGroupEndpointSpec(itemGroupId);
        return new EveEsiRequest<ApiItemGroup>(spec, entityTagProvider);
    }

    internal EveEsiRequest<ApiItemCategory> ItemCategoryRequest(int id)
    {
        var spec = new ItemCategoryEndpointSpec(id);
        return new EveEsiRequest<ApiItemCategory>(spec, entityTagProvider);
    }

    public EveEsiRequest<ApiEveServerStatus> ServerStatusRequest()
    {
        var spec = new ServerStatusEndpointSpec();
        return new EveEsiRequest<ApiEveServerStatus>(spec, entityTagProvider);
    }
}
