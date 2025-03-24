using System.Dynamic;

namespace Mantle.Web;

public static class ObjectExtensions
{
    public static IDictionary<string, object> ToDictionary(this object obj) => HtmlHelper.AnonymousObjectToHtmlAttributes(obj);

    public static ExpandoObject ToExpando(this object anonymousObject)
    {
        var anonymousDictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(anonymousObject);
        IDictionary<string, object> expando = new ExpandoObject();
        foreach (var item in anonymousDictionary)
        {
            expando.Add(item);
        }
        return (ExpandoObject)expando;
    }
}