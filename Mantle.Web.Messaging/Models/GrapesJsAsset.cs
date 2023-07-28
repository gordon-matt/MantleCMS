using Newtonsoft.Json;

namespace Mantle.Web.Messaging.Models;

public class GrapesJsAsset
{
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("src")]
    public string Source { get; set; }
}