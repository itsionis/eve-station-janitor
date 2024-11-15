using EveStationJanitor.EveApi.Esi;
using EveStationJanitor.EveApi.Status.Objects;

namespace EveStationJanitor.EveApi.Status;

public interface IStatusApi
{
    Task<EveEsiResult<ApiEveServerStatus>> GetServerStatus();
}