using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mantle.JQQueryBuilder
{
    public class JQQueryBuilderConfig
    {
        public static readonly Lazy<List<JQQueryBuilderOperatorType>> AllOperatorTypes;
        public static readonly Lazy<List<JQQueryBuilderOperatorType>> ShortTextOperatorTypes;
        public static readonly Lazy<List<JQQueryBuilderOperatorType>> LongTextOperatorTypes;
        public static readonly Lazy<List<JQQueryBuilderOperatorType>> NumericOperatorTypes;
        public static readonly Lazy<List<JQQueryBuilderOperatorType>> DateTimeOperatorTypes;
        public static readonly Lazy<List<JQQueryBuilderOperatorType>> BooleanOperatorTypes;
        public static readonly Lazy<Dictionary<string, object>> DefaultPlugins;

        static JQQueryBuilderConfig()
        {
            AllOperatorTypes = new Lazy<List<JQQueryBuilderOperatorType>>(() =>
            {
                return new List<JQQueryBuilderOperatorType>
                {
                    JQQueryBuilderOperatorType.Equal,
                    JQQueryBuilderOperatorType.NotEqual,
                    JQQueryBuilderOperatorType.In,
                    JQQueryBuilderOperatorType.NotIn,
                    JQQueryBuilderOperatorType.LessThan,
                    JQQueryBuilderOperatorType.LessThanOrEqual,
                    JQQueryBuilderOperatorType.GreaterThan,
                    JQQueryBuilderOperatorType.GreaterThanOrEqual,
                    JQQueryBuilderOperatorType.Between,
                    JQQueryBuilderOperatorType.NotBetween,
                    JQQueryBuilderOperatorType.BeginsWith,
                    JQQueryBuilderOperatorType.NotBeginsWith,
                    JQQueryBuilderOperatorType.Contains,
                    JQQueryBuilderOperatorType.NotContains,
                    JQQueryBuilderOperatorType.EndsWith,
                    JQQueryBuilderOperatorType.NotEndsWith,
                    JQQueryBuilderOperatorType.IsEmpty,
                    JQQueryBuilderOperatorType.IsNotEmpty,
                    JQQueryBuilderOperatorType.IsNull,
                    JQQueryBuilderOperatorType.IsNotNull
                };
            });
            ShortTextOperatorTypes = new Lazy<List<JQQueryBuilderOperatorType>>(() =>
            {
                return new List<JQQueryBuilderOperatorType>
                {
                    JQQueryBuilderOperatorType.Equal,
                    JQQueryBuilderOperatorType.NotEqual,
                    JQQueryBuilderOperatorType.In,
                    JQQueryBuilderOperatorType.NotIn,
                    JQQueryBuilderOperatorType.BeginsWith,
                    JQQueryBuilderOperatorType.NotBeginsWith,
                    JQQueryBuilderOperatorType.Contains,
                    JQQueryBuilderOperatorType.NotContains,
                    JQQueryBuilderOperatorType.EndsWith,
                    JQQueryBuilderOperatorType.NotEndsWith,
                    JQQueryBuilderOperatorType.IsEmpty,
                    JQQueryBuilderOperatorType.IsNotEmpty,
                    JQQueryBuilderOperatorType.IsNull,
                    JQQueryBuilderOperatorType.IsNotNull
                };
            });
            LongTextOperatorTypes = new Lazy<List<JQQueryBuilderOperatorType>>(() =>
            {
                return new List<JQQueryBuilderOperatorType>
                {
                    JQQueryBuilderOperatorType.BeginsWith,
                    JQQueryBuilderOperatorType.NotBeginsWith,
                    JQQueryBuilderOperatorType.Contains,
                    JQQueryBuilderOperatorType.NotContains,
                    JQQueryBuilderOperatorType.EndsWith,
                    JQQueryBuilderOperatorType.NotEndsWith,
                    JQQueryBuilderOperatorType.IsEmpty,
                    JQQueryBuilderOperatorType.IsNotEmpty,
                    JQQueryBuilderOperatorType.IsNull,
                    JQQueryBuilderOperatorType.IsNotNull
                };
            });
            NumericOperatorTypes = new Lazy<List<JQQueryBuilderOperatorType>>(() =>
            {
                return new List<JQQueryBuilderOperatorType>
                {
                    JQQueryBuilderOperatorType.Equal,
                    JQQueryBuilderOperatorType.NotEqual,
                    JQQueryBuilderOperatorType.In,
                    JQQueryBuilderOperatorType.NotIn,
                    JQQueryBuilderOperatorType.LessThan,
                    JQQueryBuilderOperatorType.LessThanOrEqual,
                    JQQueryBuilderOperatorType.GreaterThan,
                    JQQueryBuilderOperatorType.GreaterThanOrEqual,
                    JQQueryBuilderOperatorType.Between,
                    JQQueryBuilderOperatorType.NotBetween,
                    JQQueryBuilderOperatorType.IsNull,
                    JQQueryBuilderOperatorType.IsNotNull
                };
            });
            DateTimeOperatorTypes = new Lazy<List<JQQueryBuilderOperatorType>>(() =>
            {
                return new List<JQQueryBuilderOperatorType>
                {
                    JQQueryBuilderOperatorType.Equal,
                    JQQueryBuilderOperatorType.NotEqual,
                    JQQueryBuilderOperatorType.In,
                    JQQueryBuilderOperatorType.NotIn,
                    JQQueryBuilderOperatorType.LessThan,
                    JQQueryBuilderOperatorType.LessThanOrEqual,
                    JQQueryBuilderOperatorType.GreaterThan,
                    JQQueryBuilderOperatorType.GreaterThanOrEqual,
                    JQQueryBuilderOperatorType.Between,
                    JQQueryBuilderOperatorType.NotBetween,
                    JQQueryBuilderOperatorType.IsNull,
                    JQQueryBuilderOperatorType.IsNotNull
                };
            });
            BooleanOperatorTypes = new Lazy<List<JQQueryBuilderOperatorType>>(() =>
            {
                return new List<JQQueryBuilderOperatorType>
                {
                    JQQueryBuilderOperatorType.Equal,
                    JQQueryBuilderOperatorType.IsNull,
                    JQQueryBuilderOperatorType.IsNotNull
                };
            });
            DefaultPlugins = new Lazy<Dictionary<string, object>>(() =>
            {
                return new Dictionary<string, object>
                {
                    { "sortable", null },
                    { "filter-description", null },
                    { "unique-filter", null },
                    { "bt-tooltip-errors", null },
                    { "bt-selectpicker", null },
                    { "bt-checkbox", null },
                    { "invert", null },
                    { "not-group", null },
                    { "sql-support", JObject.FromObject(new { boolean_as_integer = false }) }
                };
            });
        }

        [JsonProperty("plugins")]
        public Dictionary<string, object> Plugins { get; set; }

        [JsonProperty("filters")]
        public IEnumerable<JQQueryBuilderFilter> Filters { get; set; }

        [JsonProperty("rules")]
        public JQQueryBuilderRuleSet Rules { get; set; }
    }
}