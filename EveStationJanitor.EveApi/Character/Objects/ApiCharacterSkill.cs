using System.Text.Json.Serialization;

namespace EveStationJanitor.EveApi.Character.Objects;

public class ApiCharacterSkill
{
    [JsonPropertyName("active_skill_level")]
    public int ActiveSkillLevel { get; set; }

    [JsonPropertyName("skill_id")]
    public int SkillId { get; set; }

    [JsonPropertyName("skillpoints_in_skill")]
    public long SkillPointsInSkill { get; set; }

    [JsonPropertyName("trained_skill_level")]
    public int TrainedSkillLevel { get; set; }
}
