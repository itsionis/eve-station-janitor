using System.Text.Json.Serialization;

namespace EveStationJanitor.EveApi.Status.Objects;

public class ApiEveServerStatus
{
    [JsonPropertyName("players")]
    [JsonRequired]
    public required int PlayerCount { get; set; }
    
    [JsonPropertyName("server_version")]
    [JsonRequired]
    public required string ServerVersion { get; set; }
    
    [JsonPropertyName("start_time")]
    [JsonRequired]
    public required DateTime StartTime { get; set; }
    
    [JsonPropertyName("vip")]
    public bool? IsVipMode { get; set; }
}