using System.ComponentModel;

namespace Mantle.Web.Mvc.KendoUI;

[Browsable(false)]
[EditorBrowsable(EditorBrowsableState.Never)]
internal static class TypeExtensions
{
    internal static readonly Type[] PredefinedTypes =
    {
        typeof(object),
        typeof(bool),
        typeof(char),
        typeof(string),
        typeof(sbyte),
        typeof(byte),
        typeof(short),
        typeof(ushort),
        typeof(int),
        typeof(uint),
        typeof(long),
        typeof(ulong),
        typeof(float),
        typeof(double),
        typeof(decimal),
        typeof(DateTime),
        typeof(TimeSpan),
        typeof(Guid),
        typeof(Math),
        typeof(Convert)
};

    public static bool IsPredefinedType(this Type type)
    {
        return PredefinedTypes.Any(left => left == type);
    }

    public static string FirstSortableProperty(this Type type)
    {
        var propertyInfo = type.GetTypeInfo().GetProperties().FirstOrDefault(property => property.PropertyType.IsPredefinedType());

        if (propertyInfo == null)
        {
            throw new NotSupportedException("CannotFindPropertyToSortBy");
        }

        return propertyInfo.Name;
    }
}