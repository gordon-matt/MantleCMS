using Extenso;
using System.Dynamic;
using System.Text;

namespace Mantle.Web
{
    public static class ExpandoObjectExtensions
    {
        public static string ToJson(this ExpandoObject expando)
        {
            var json = new StringBuilder();
            var keyPairs = new List<string>();
            IDictionary<string, object> dictionary = expando;
            json.Append("{");

            foreach (var pair in dictionary)
            {
                if (pair.Value is ExpandoObject)
                {
                    keyPairs.Add(string.Format(@"""{0}"": {1}", pair.Key, (pair.Value as ExpandoObject).ToJson()));
                }
                else
                {
                    keyPairs.Add(string.Format(@"""{0}"": {1}", pair.Key, pair.Value.JsonSerialize()));
                }
            }

            json.Append(string.Join(",", keyPairs.ToArray()));
            json.Append("}");

            return json.ToString();
        }
    }
}