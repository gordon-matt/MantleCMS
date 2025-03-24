using System.Linq.Dynamic.Core;

namespace Mantle.Web.Mvc.KendoUI;

public static class Extensions
{
    public class DataItem
    {
        public string Fieldname { get; set; }
        public string Prefix { get; set; }
        public object Value { get; set; }
    }

    public static IEnumerable<DataItem> GetDataItems(this DynamicClass self, string propertyName)
    {
        var propertyType = self.GetType();
        var propertyInfo = propertyType.GetProperty(propertyName);

        if (propertyInfo == null)
        {
            return [];
        }

        object property = propertyInfo.GetValue(self, null);
        var props = property.GetType().GetProperties().Where(p => p.Name.Contains("__"));

        return props
            // Split on __ to get the prefix and the field
            .Select(prop => new { PropertyInfo = prop, Data = prop.Name.Split(new[] { "__" }, StringSplitOptions.None) })

            // Return the Fieldname, Prefix and the the value ('First' , 'field' , 'First')
            .Select(x => new DataItem { Fieldname = x.Data.Last(), Prefix = x.Data.First(), Value = x.PropertyInfo.GetValue(property, null) })
        ;
    }

    public static object GetAggregatesAsDictionary(this DynamicClass self)
    {
        var dataItems = self.GetDataItems("Aggregates");

        // Group by the field and return an anonymous dictionary
        return dataItems
            .GroupBy(groupBy => groupBy.Fieldname)
            .ToDictionary(x => x.Key, y => y.ToDictionary(k => k.Prefix, v => v.Value));
    }

    public static IDictionary<string, object> ToDictionary(this object a)
    {
        var type = a.GetType();
        var props = type.GetProperties();
        return props.ToDictionary(x => x.Name, y => y.GetValue(a, null));
    }
}