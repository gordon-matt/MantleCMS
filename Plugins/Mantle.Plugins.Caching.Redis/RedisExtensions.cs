using System.Reflection;
using Extenso.Reflection;
using StackExchange.Redis;

namespace Mantle.Plugins.Caching.Redis;

public static class RedisExtensions
{
    extension<T>(T obj)
    {
        public HashEntry[] ToHashEntries()
        {
            var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => x.CanWrite);

            return properties
                .Where(x => x.GetValue(obj) != null) // <-- PREVENT NullReferenceException
                .Select(property => new HashEntry(property.Name, property.GetValue(obj).ToString()))
                .ToArray();
        }
    }

    extension(HashEntry[] hashEntries)
    {
        public T ConvertFromRedis<T>()
        {
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => x.CanWrite);
            object obj = Activator.CreateInstance<T>();
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
    }

    extension(IDatabase database)
    {
        public void KeyDeleteByPattern(string prefix)
        {
            ArgumentNullException.ThrowIfNull(database, nameof(database));
            ArgumentException.ThrowIfNullOrWhiteSpace(prefix, nameof(prefix));

            database.ScriptEvaluate(@"
            local keys = redis.call('keys', ARGV[1])
            for i=1,#keys,5000 do
            redis.call('del', unpack(keys, i, math.min(i+4999, #keys)))
            end", values: [prefix]);
        }
    }
}