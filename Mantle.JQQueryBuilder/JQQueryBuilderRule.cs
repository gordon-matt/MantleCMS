using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Mantle.JQQueryBuilder
{
    public class JQQueryBuilderRule : IJQQueryBuilderRule
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("field")]
        public string Field { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("type")]
        public JQQueryBuilderFieldType Type { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("input")]
        public JQQueryBuilderInputType Input { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("operator")]
        public JQQueryBuilderOperatorType Operator { get; set; }

        [JsonProperty("value")]
        public object Value { get; set; }
    }
}