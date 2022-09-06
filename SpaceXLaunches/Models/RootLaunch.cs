using Newtonsoft.Json;

namespace SpaceXLaunches.Models
{
    public class RootLaunch
    {
        [JsonProperty(PropertyName = "date_utc")]
        public DateTime date_utc { get; set; }

        [JsonProperty(PropertyName = "success")]
        public string? success { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string name { get; set; }

    }
}
