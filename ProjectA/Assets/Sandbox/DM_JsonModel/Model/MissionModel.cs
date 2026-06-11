using Newtonsoft.Json;

namespace JsonModel
{
    public class MissionModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("mission_type")]
        public string MissionType { get; set; }

        [JsonProperty("required_count")]
        public int RequiredCount { get; set; }

        [JsonProperty("reward")]
        public Reward Reward { get; set; }

        [JsonProperty("is_active")]
        public bool IsActive { get; set; }

    }
}
