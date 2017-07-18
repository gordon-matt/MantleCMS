using System.Collections.Generic;
using System.Dynamic;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;

namespace Mantle.Web
{
    public static class ObjectExtensions
    {
        public static string ToJson<T>(this T item, JsonSerializerSettings settings = null)
        {
            if (item == null)
            {
                return null;
            }

            if (settings == null)
            {
                return JsonConvert.SerializeObject(item);
            }

            return JsonConvert.SerializeObject(item, settings);
        }

        public static IDictionary<string, object> ToDictionary(this object obj)
        {
            return HtmlHelper.AnonymousObjectToHtmlAttributes(obj);
        }

        public static ExpandoObject ToExpando(this object anonymousObject)
        {
            IDictionary<string, object> anonymousDictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(anonymousObject);
            IDictionary<string, object> expando = new ExpandoObject();
            foreach (var item in anonymousDictionary)
            {
                expando.Add(item);
            }
            return (ExpandoObject)expando;
        }
    }
}