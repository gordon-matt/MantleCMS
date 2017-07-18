using System.Collections.Specialized;
using System.Linq;
using Microsoft.AspNetCore.Routing;

namespace Mantle.Web.Mvc.Routing
{
    public static class RouteValueDictionaryExtensions
    {
        public static RouteValueDictionary Merge(this RouteValueDictionary obj, object values, params string[] removeKeys)
        {
            var mergeValues = new RouteValueDictionary(values);

            var result = new RouteValueDictionary(obj);
            foreach (var value in mergeValues)
            {
                result[value.Key.Replace("_", "-")] = value.Value;
            }

            if (removeKeys != null && removeKeys.Length > 0)
            {
                foreach (var key in removeKeys.Where(result.ContainsKey))
                {
                    result.Remove(key);
                }
            }

            return result;
        }

        public static RouteValueDictionary Merge(this RouteValueDictionary obj, NameValueCollection values, params string[] removeKeys)
        {
            var result = new RouteValueDictionary(obj);
            foreach (var key in values.AllKeys)
            {
                result[key] = values[key];
            }

            if (removeKeys != null && removeKeys.Length > 0)
            {
                foreach (var key in removeKeys.Where(result.ContainsKey))
                {
                    result.Remove(key);
                }
            }

            return result;
        }
    }
}