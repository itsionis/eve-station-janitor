using System.Text.Json.Serialization;

namespace EveStationJanitor.EveApi.Market.Objects;

public class ApiMarketOrder
{
    [JsonPropertyName("duration")]
    public int Duration { get; set; }

    [JsonPropertyName("is_buy_order")]
    public bool IsBuyOrder { get; set; }

    [JsonPropertyName("issued")]
    public string Issued { get; set; } = string.Empty;

    [JsonPropertyName("location_id")]
    public long LocationId { get; set; }

    [JsonPropertyName("min_volume")]
    public int MinVolume { get; set; }

    [JsonPropertyName("order_id")]
    public long OrderId { get; set; }

    [JsonPropertyName("price")]
    public double Price { get; set; }

    [JsonPropertyName("range")]
    public string Range { get; set; } = string.Empty;

    [JsonPropertyName("system_id")]
    public int SystemId { get; set; }

    [JsonPropertyName("type_id")]
    public int TypeId { get; set; }

    [JsonPropertyName("volume_remain")]
    public int VolumeRemaining { get; set; }

    [JsonPropertyName("volume_total")]
    public int VolumeTotal { get; set; }
}
