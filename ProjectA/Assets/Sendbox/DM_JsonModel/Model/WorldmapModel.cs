using Newtonsoft.Json;

namespace JsonModel
{
    public class WorldmapModel
    {
        [JsonProperty("image")] public string Image { get; set; }
        [JsonProperty("type")] public string Type { get; set; }
        [JsonProperty("position")] public Position Position { get; set; }
        [JsonProperty("timer")] public Timer Timer { get; set; }
    }
}
