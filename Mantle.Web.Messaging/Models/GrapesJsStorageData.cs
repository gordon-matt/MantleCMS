using Newtonsoft.Json;

namespace Mantle.Web.Messaging.Models
{
    public class GrapesJsStorageData
    {
        [JsonProperty("gjs-assets")]
        public string Assets { get; set; }

        [JsonProperty("gjs-css")]
        public string Css { get; set; }

        [JsonProperty("gjs-styles")]
        public string Styles { get; set; }

        [JsonProperty("gjs-html")]
        public string Html { get; set; }

        [JsonProperty("gjs-components")]
        public string Components { get; set; }
    }
}