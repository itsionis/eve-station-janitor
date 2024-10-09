using EveStationJanitor.Authentication;
using EveStationJanitor.EveApi.Character;
using EveStationJanitor.EveApi.Character.Objects;
using EveStationJanitor.EveApi.Clone;
using EveStationJanitor.EveApi.Clone.Objects;
using EveStationJanitor.EveApi.Market;
using EveStationJanitor.EveApi.Market.Objects;
using EveStationJanitor.EveApi.Universe;
using EveStationJanitor.EveApi.Universe.Objects;

namespace EveStationJanitor.EveApi;

internal class EveEsiRequestFactory(ITokenProvider tokenProvider, IEntityTagProvider entityTagProvider)
{
    public EveEsiRequest<ApiCloneImplants> CloneImplantsRequest(int characterId)
    {
        var spec = new CloneImplantsEndpointSpec(characterId);
        return new EveEsiRequest<ApiCloneImplants>(spec, entityTagProvider, tokenProvider);
    }

    public EveEsiRequest<ApiCharacterSkills> CharacterSkillsRequest(int characterId)
    {
        var spec = new CharacterSkillsEndpointSpec(characterId);
        return new EveEsiRequest<ApiCharacterSkills>(spec, entityTagProvider, tokenProvider);
    }

    public EveEsiRequest<List<ApiCharacterStanding>> CharacterStandingsRequest(int characterId)
    {
        var spec = new CharacterStandingsEndpointSpec(characterId);
        return new EveEsiRequest<List<ApiCharacterStanding>>(spec, entityTagProvider, tokenProvider);
    }

    public EveEsiPagedRequest<ApiMarketOrder> MarketOrdersRequest(int regionId, int? itemTypeId, ApiMarketOrderType orderType)
    {
        var spec = new MarketOrdersEndpointSpec(regionId, itemTypeId, orderType);
        return new EveEsiPagedRequest<ApiMarketOrder>(spec, entityTagProvider, tokenProvider);
    }

    public EveEsiRequest<ApiItemType> ItemTypeRequest(int itemId)
    {
        var spec = new ItemTypeEndpointSpec(itemId);
        return new EveEsiRequest<ApiItemType>(spec, entityTagProvider, tokenProvider);
    }

    public EveEsiRequest<ApiItemGroup> ItemGroupRequest(int itemGroupId)
    {
        var spec = new ItemGroupEndpointSpec(itemGroupId);
        return new EveEsiRequest<ApiItemGroup>(spec, entityTagProvider, tokenProvider);
    }

    internal EveEsiRequest<ApiItemCategory> ItemCategoryRequest(int id)
    {
        var spec = new ItemCategoryEndpointSpec(id);
        return new EveEsiRequest<ApiItemCategory>(spec, entityTagProvider, tokenProvider);
    }
}
