using System.Text.Json.Serialization;

namespace EveStationJanitor.EveApi.Character.Objects;

public class ApiCharacterSkills
{
    [JsonPropertyName("skills")]
    public List<ApiCharacterSkill> Skills { get; set; } = [];

    [JsonPropertyName("total_sp")]
    public long TotalSkillPoints { get; set; }

    [JsonPropertyName("unallocated_sp")]
    public int? UnallocatedSkillPoints { get; set; }
}
