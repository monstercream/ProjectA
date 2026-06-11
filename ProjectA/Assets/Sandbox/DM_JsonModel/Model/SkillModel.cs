using Newtonsoft.Json;

namespace JsonModel
{
    public class SkillModel
    {
        [JsonProperty("image")] public string Image { get; set; }
        [JsonProperty("type")] public string Type { get; set; }
        [JsonProperty("statInfo")] public StatInfo StatInfo { get; set; }
    }
}
