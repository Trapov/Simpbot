using Newtonsoft.Json;

namespace Simpbot.Service.Weather.Contract
{
    public class Clouds
    {
        [JsonProperty("all")]
        public long All { get; set; }
    }
}