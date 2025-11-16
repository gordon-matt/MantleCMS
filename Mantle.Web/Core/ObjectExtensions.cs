using System.Dynamic;

namespace Mantle.Web;

public static class ObjectExtensions
{
    extension(object source)
    {
        public IDictionary<string, object> ToDictionary() =>
            HtmlHelper.AnonymousObjectToHtmlAttributes(source);

        public ExpandoObject ToExpando()
        {
            var anonymousDictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(source);
            IDictionary<string, object> expando = new ExpandoObject();
            foreach (var item in anonymousDictionary)
            {
                expando.Add(item);
            }
            return (ExpandoObject)expando;
        }
    }
}