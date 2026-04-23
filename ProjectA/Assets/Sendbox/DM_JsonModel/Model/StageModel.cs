using Newtonsoft.Json;

namespace JsonModel
{
    public class StageModel
    {
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("prefab_path")] public string PrefabPath { get; set; }
        [JsonProperty("image_path")] public string ImagePath { get; set; }
        
        [JsonProperty("start_position")] public Vector3Json StartPosition { get; set; }
    }
}
