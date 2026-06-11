using Newtonsoft.Json;

namespace JsonModel
{
    public class LevelModel
    {
        [JsonProperty("level")] public string Level { get; set; }
        [JsonProperty("needExp")] public string NeedExp { get; set; }
        [JsonProperty("totalExp")] public string TotalExp { get; set; }
        [JsonProperty("exponentialRate")] public string ExponentialRate { get; set; }
        [JsonProperty("grade")] public string Grade { get; set; }
        [JsonProperty("bundleKey")] public string BundleKey { get; set; }

    }
}
