using System;
using System.Linq;
using System.Reflection;

namespace Mantle.Reflection
{
    public static class TypeExtensions
    {
        private static readonly Lazy<Type[]> simpleTypes;

        static TypeExtensions()
        {
            simpleTypes = new Lazy<Type[]>(() =>
            {
                var types = new[]
                {
                    typeof(Boolean),
                    typeof(Byte),
                    typeof(Char),
                    typeof(DateTime),
                    typeof(DateTimeOffset),
                    typeof(Decimal),
                    typeof(Double),
                    typeof(Enum),
                    typeof(Guid),
                    typeof(Int16),
                    typeof(Int32),
                    typeof(Int64),
                    typeof(IntPtr),
                    typeof(SByte),
                    typeof(Single),
                    typeof(String),
                    typeof(TimeSpan),
                    typeof(UInt16),
                    typeof(UInt32),
                    typeof(UInt64),
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
}