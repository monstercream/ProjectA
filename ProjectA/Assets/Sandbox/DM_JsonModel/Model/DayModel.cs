using Newtonsoft.Json;

namespace JsonModel
{
    public class DayModel
    {
        [JsonProperty("image")]
        public string Image { get; set; }

        [JsonProperty("day")]
        public int Day { get; set; }

        [JsonProperty("bundleKey")]
        public string BundleKey { get; set; }

    }
}
