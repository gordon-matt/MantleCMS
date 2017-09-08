//using Newtonsoft.Json;
//using Newtonsoft.Json.Converters;

//namespace Mantle.JQQueryBuilder
//{
//    public class JQQueryBuilderOperator
//    {
//        public JQQueryBuilderOperator()
//        {
//            NumberOfInputs = 1;
//            Multiple = false;
//            ApplyTo = new string[] { "string", "number", "datetime", "boolean" };
//        }

//        /// <summary>
//        /// Identifier of the operator, use the lang.operators to translate or give a human readable name to your operator.
//        /// </summary>
//        [JsonConverter(typeof(StringEnumConverter))]
//        [JsonProperty("type")]
//        public JQQueryBuilderOperatorType Type { get; set; }

//        /// <summary>
//        /// <para>Optional id of an&lt;optgroup&gt; in the operators dropdown. If the optgroup does not exist in the global optgroups map,</para>
//        /// <para>it will be created with label=id for all languages.</para>
//        /// </summary>
//        [JsonProperty("optgroup")]
//        public string OptionGroup { get; set; }

//        /// <summary>
//        /// The number of inputs displayed. Typical values are 0 (is_null & similar operators), 1 (most operators) and 2 (between operator).
//        /// </summary>
//        [JsonProperty("nb_inputs")]
//        public int NumberOfInputs { get; set; }

//        /// <summary>
//        /// Inform the builder that each input can have multiple values. true for in, bot_in, false otherwise.
//        /// </summary>
//        [JsonProperty("multiple")]
//        public bool Multiple { get; set; }

//        /// <summary>
//        /// An array containing string, number, datetime, boolean.
//        /// </summary>
//        [JsonProperty("apply_to")]
//        public string[] ApplyTo { get; set; }
//    }
//}