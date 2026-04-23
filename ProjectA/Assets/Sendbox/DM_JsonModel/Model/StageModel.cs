using Newtonsoft.Json;

namespace JsonModel
{
    public class StageModel
    {
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("path")] public string Path { get; set; }
    }
}
