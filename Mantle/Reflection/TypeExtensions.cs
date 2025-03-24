using System.Reflection;

namespace Mantle.Reflection;

public static class TypeExtensions
{
    private static readonly Lazy<Type[]> simpleTypes;

    static TypeExtensions()
    {
        simpleTypes = new Lazy<Type[]>(() =>
        {
            var types = new[]
            {
                typeof(bool),
                typeof(byte),
                typeof(char),
                typeof(DateTime),
                typeof(DateTimeOffset),
                typeof(decimal),
                typeof(double),
                typeof(Enum),
                typeof(Guid),
                typeof(short),
                typeof(int),
                typeof(long),
                typeof(IntPtr),
                typeof(sbyte),
                typeof(float),
                typeof(string),
                typeof(TimeSpan),
                typeof(ushort),
                typeof(uint),
                typeof(ulong),
                typeof(UIntPtr),
                typeof(Uri)
            };

            var nullTypes = types
                .Where(t => t.GetTypeInfo().IsValueType)
                .Select(t => typeof(Nullable<>).MakeGenericType(t));

            return types.Concat(nullTypes).ToArray();
        });
    }

    public static bool IsSimple(this Type type)
    {
        if (type.GetTypeInfo().IsPrimitive || simpleTypes.Value.Any(x => x.GetTypeInfo().IsAssignableFrom(type)))
        {
            return true;
        }

        var nut = Nullable.GetUnderlyingType(type);
        return nut != null && nut.GetTypeInfo().IsEnum;
    }
}