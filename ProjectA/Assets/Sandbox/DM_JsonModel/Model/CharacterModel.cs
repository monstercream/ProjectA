using Newtonsoft.Json;
using Sirenix.OdinInspector;

namespace JsonModel
{
    [ShowInInspector]
    public class CharacterModel
    {
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("equipments")] public Equipments Equipments { get; set; }
    }

    [ShowInInspector]
    public class Equipments
    {
        [JsonProperty("right")] public string Right { get; set; }
        [JsonProperty("head")] public string Head { get; set; }
        [JsonProperty("body")] public string Body { get; set; }
    }


    [ShowInInspector]
    public class StatInfo
    {
        [JsonProperty("health")] public string Health { get; set; }
        [JsonProperty("mana")] public string Mana { get; set; }
        [JsonProperty("attack")] public string Attack { get; set; }
        [JsonProperty("defence")] public string Defence { get; set; }
        [JsonProperty("speed")] public string Speed { get; set; }
        [JsonProperty("intelligence")] public string Intelligence { get; set; }
    }

    public class Reward
    {
        [JsonProperty("type")] public string Type { get; set; }
        [JsonProperty("amount")] public string Amount { get; set; }
    }

    public class Position
    {
        [JsonProperty("x")] public string X { get; set; }
        [JsonProperty("y")] public string Y { get; set; }
        [JsonProperty("z")] public string Z { get; set; }
    }

    public class Timer
    {
        [JsonProperty("instance_id")] public string ID { get; set; }
        [JsonProperty("start_time")] public string StartTime { get; set; }
        [JsonProperty("time")] public string Time { get; set; }
    }
}