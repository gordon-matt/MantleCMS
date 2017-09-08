using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Mantle.JQQueryBuilder
{
    public class JQQueryBuilderFilter
    {
        public JQQueryBuilderFilter()
        {
            Type = JQQueryBuilderFieldType.String;
            Input = JQQueryBuilderInputType.Text;
            InputEvent = "change";
            Multiple = false;
            Vertical = false;
            //Operators = new string[] { "equal" };
            Operators = new List<JQQueryBuilderOperatorType>
            {
                JQQueryBuilderOperatorType.Equal
            };
        }

        /// <summary>
        /// Unique identifier of the filter.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Field used by the filter, multiple filters can use the same field.
        /// </summary>
        [JsonProperty("field")]
        public string Field { get; set; }

        /// <summary>
        /// Label used to display the filter. It can be simple string or a map for localization.
        /// </summary>
        [JsonProperty("label")]
        public string Label { get; set; }

        /// <summary>
        /// <para>Optional id of an &lt;optgroup&gt; in the filters dropdown. If the optgroup does not exist in the global optgroups map,</para>
        /// <para>it will be created with label=id for all languages.</para>
        /// </summary>
        [JsonProperty("optgroup")]
        public string OptionGroup { get; set; }

        /// <summary>
        /// Type of the field. Available types are string, integer, double, date, time, datetime and boolean.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("type")]
        public JQQueryBuilderFieldType Type { get; set; }

        /// <summary>
        /// <para>Type of input used. Available types are text, textarea, radio, checkbox and select.</para>
        /// <para>It can also be a function which returns the HTML of the said input, this function takes 2 parameters:</para>
        /// <para>- rule a Rule object</para>
        /// <para>- input_name the name of the input</para>
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("input")]
        public JQQueryBuilderInputType Input { get; set; }

        /// <summary>
        /// Required for radio and checkbox inputs. Generally needed for select inputs.
        /// </summary>
        [JsonProperty("values")]
        public object Values { get; set; }

        /// <summary>
        /// <para>Used the split and join the value when a text input is used with an operator allowing multiple values</para>
        /// <para>(between for example).</para>
        /// </summary>
        [JsonProperty("value_separator")]
        public string ValueSeparator { get; set; }

        /// <summary>
        /// As it says :-)
        /// </summary>
        [JsonProperty("default_value")]
        public object DefaultValue { get; set; }

        /// <summary>
        /// Space separated list of DOM events which the builder should listen to detect value changes.
        /// </summary>
        [JsonProperty("input_event")]
        public string InputEvent { get; set; }

        /// <summary>
        /// Only for text and textarea inputs: horizontal size of the input.
        /// </summary>
        [JsonProperty("size")]
        public int Size { get; set; }

        /// <summary>
        /// Only for textarea inputs: vertical size of the input.
        /// </summary>
        [JsonProperty("rows")]
        public int Rows { get; set; }

        /// <summary>
        /// Only for select inputs: accept multiple values.
        /// </summary>
        [JsonProperty("multiple")]
        public bool Multiple { get; set; }

        /// <summary>
        /// Only for text and textarea inputs: placeholder to display inside the input.
        /// </summary>
        [JsonProperty("placeholder")]
        public string Placeholder { get; set; }

        /// <summary>
        /// Only for radio and checkbox inputs: display inputs vertically on not horizontally.
        /// </summary>
        [JsonProperty("vertical")]
        public bool Vertical { get; set; }

        [JsonProperty("validation")]
        public object Validation { get; set; }

        /// <summary>
        /// Array of operators types to use for this filter. If empty the filter will use all applicable operators.
        /// </summary>
        [JsonIgnore]
        public List<JQQueryBuilderOperatorType> Operators { get; set; }

        [JsonProperty("operators")]
        public string[] OperatorsInternal
        {
            get
            {
                var operators = new List<string>();
                foreach (var op in Operators)
                {
                    switch (op)
                    {
                        case JQQueryBuilderOperatorType.Equal: operators.Add("equal"); break;
                        case JQQueryBuilderOperatorType.NotEqual: operators.Add("not_equal"); break;
                        case JQQueryBuilderOperatorType.In: operators.Add("in"); break;
                        case JQQueryBuilderOperatorType.NotIn: operators.Add("not_in"); break;
                        case JQQueryBuilderOperatorType.LessThan: operators.Add("less"); break;
                        case JQQueryBuilderOperatorType.LessThanOrEqual: operators.Add("less_or_equal"); break;
                        case JQQueryBuilderOperatorType.GreaterThan: operators.Add("greater"); break;
                        case JQQueryBuilderOperatorType.GreaterThanOrEqual: operators.Add("greater_or_equal"); break;
                        case JQQueryBuilderOperatorType.Between: operators.Add("between"); break;
                        case JQQueryBuilderOperatorType.NotBetween: operators.Add("not_between"); break;
                        case JQQueryBuilderOperatorType.BeginsWith: operators.Add("begins_with"); break;
                        case JQQueryBuilderOperatorType.NotBeginsWith: operators.Add("not_begins_with"); break;
                        case JQQueryBuilderOperatorType.Contains: operators.Add("contains"); break;
                        case JQQueryBuilderOperatorType.NotContains: operators.Add("not_contains"); break;
                        case JQQueryBuilderOperatorType.EndsWith: operators.Add("ends_with"); break;
                        case JQQueryBuilderOperatorType.NotEndsWith: operators.Add("not_ends_with"); break;
                        case JQQueryBuilderOperatorType.IsEmpty: operators.Add("is_empty"); break;
                        case JQQueryBuilderOperatorType.IsNotEmpty: operators.Add("is_not_empty"); break;
                        case JQQueryBuilderOperatorType.IsNull: operators.Add("is_null"); break;
                        case JQQueryBuilderOperatorType.IsNotNull: operators.Add("is_not_null"); break;
                    }
                }
                return operators.ToArray();
            }
            set { }
        }

        /// <summary>
        /// Name of a jQuery plugin to apply on the input.
        /// </summary>
        [JsonProperty("plugin")]
        public string Plugin { get; set; }

        /// <summary>
        /// Object of parameters to pass to the plugin.
        /// </summary>
        [JsonProperty("plugin_config")]
        public object PluginConfig { get; set; }

        /// <summary>
        /// <para>Additional data not used by QueryBuilder but that will be added to the output rules object.</para>
        /// <para>Use this to store any functional data you need.</para>
        /// </summary>
        [JsonProperty("data")]
        public object Data { get; set; }

        [JsonProperty("unique")]
        public bool Unique { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("colors")]
        public object Colors { get; set; }
    }
}