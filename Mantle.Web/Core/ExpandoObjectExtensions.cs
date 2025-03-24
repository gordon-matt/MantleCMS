using System.Dynamic;

namespace Mantle.Web;

public static class ExpandoObjectExtensions
{
    public static string ToJson(this ExpandoObject expando)
    {
        var json = new StringBuilder();
        var keyPairs = new List<string>();
        IDictionary<string, object> dictionary = expando;
        json.Append('{');

        foreach (var pair in dictionary)
        {
            if (pair.Value is ExpandoObject)
            {
                keyPairs.Add($@"""{pair.Key}"": {(pair.Value as ExpandoObject).ToJson()}");
            }
            else
            {
                keyPairs.Add($@"""{pair.Key}"": {pair.Value.JsonSerialize()}");
            }
        }

        json.Append(string.Join(",", keyPairs.ToArray()));
        json.Append('}');

        return json.ToString();
    }
}