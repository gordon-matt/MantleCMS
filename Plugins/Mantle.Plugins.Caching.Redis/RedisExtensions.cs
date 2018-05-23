using System;
using System.Linq;
using System.Reflection;
using Extenso.Reflection;
using StackExchange.Redis;

namespace Mantle.Plugins.Caching.Redis
{
    public static class RedisExtensions
    {
        public static HashEntry[] ToHashEntries<T>(this T obj)
        {
            var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => x.CanWrite);

            return properties
                .Where(x => x.GetValue(obj) != null) // <-- PREVENT NullReferenceException
                .Select(property => new HashEntry(property.Name, property.GetValue(obj).ToString()))
                .ToArray();
        }

        public static T ConvertFromRedis<T>(this HashEntry[] hashEntries)
        {
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => x.CanWrite);
            var obj = Activator.CreateInstance(typeof(T));
            foreach (var property in properties)
            {
                var entry = hashEntries.FirstOrDefault(g => g.Name.ToString().Equals(property.Name));

                if (entry.Equals(new HashEntry()))
                {
                    continue;
                }

                obj.SetPropertyValue(property, entry.Value);
            }
            return (T)obj;
        }

        public static void KeyDeleteByPattern(this IDatabase database, string prefix)
        {
            if (database == null)
            {
                throw new ArgumentException("Database cannot be null", "database");
            }

            if (string.IsNullOrWhiteSpace(prefix))
            {
                throw new ArgumentException("Prefix cannot be empty", "database");
            }

            database.ScriptEvaluate(@"
                local keys = redis.call('keys', ARGV[1])
                for i=1,#keys,5000 do
                redis.call('del', unpack(keys, i, math.min(i+4999, #keys)))
                end", values: new RedisValue[] { prefix });
        }
    }
}