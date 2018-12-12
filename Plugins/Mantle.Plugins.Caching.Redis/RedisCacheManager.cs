using System;
using System.Collections;
using Extenso;
using Extenso.Reflection;
using Mantle.Caching;
using Mantle.Infrastructure;
using StackExchange.Redis;

namespace Mantle.Plugins.Caching.Redis
{
    public class RedisCacheManager : ICacheManager
    {
        public const string CacheKeyPrefix = "Mantle.Plugins.Caching.Redis.CacheKey";
        private readonly string connectionString;
        private ConnectionMultiplexer connection;

        public RedisCacheManager()
        {
            //TODO: Test
            connectionString = EngineContext.Current.Resolve<RedisCacheSettings>().ConnectionString;
        }

        ~RedisCacheManager()
        {
            if (connection != null)
            {
                if (connection.IsConnected)
                {
                    connection.Close();
                }
                connection.Dispose();
            }
        }

        protected IDatabase Database
        {
            get
            {
                if (connection == null)
                {
                    connection = GetConnection();
                }
                return connection.GetDatabase();
            }
        }

        public T Get<T>(string key)
        {
            string cacheKey = string.Concat(CacheKeyPrefix, ":Cache:", key);

            var typeofT = typeof(T);
            if (typeofT == typeof(string))
            {
                var result = Database.StringGet(cacheKey);
                if (result.HasValue)
                {
                    return (T)Convert.ChangeType(result, typeof(T));
                }
                return default(T);
            }
            else if (typeofT.IsCollection())
            {
                var result = Database.StringGet(cacheKey);
                if (result.HasValue)
                {
                    string json = result;
                    return json.JsonDeserialize<T>();
                }
                return default(T);
            }
            else
            {
                return Database.HashGetAll(cacheKey).ConvertFromRedis<T>();
            }
        }

        public void Set(string key, object data, int cacheTimeInMinutes)
        {
            string cacheKey = string.Concat(CacheKeyPrefix, ":Cache:", key);

            if (data is string)
            {
                Database.StringSet(cacheKey, (data as string), TimeSpan.FromMinutes(cacheTimeInMinutes));
            }
            else if (data is IEnumerable)
            {
                string json = data.JsonSerialize();
                Database.StringSet(cacheKey, json, TimeSpan.FromMinutes(cacheTimeInMinutes));
            }
            else
            {
                Database.HashSet(cacheKey, data.ToHashEntries());
                Database.KeyExpire(cacheKey, TimeSpan.FromMinutes(cacheTimeInMinutes));
            }
        }

        public bool IsSet(string key)
        {
            string cacheKey = string.Concat(CacheKeyPrefix, ":Cache:", key);
            return Database.KeyExists(cacheKey);
        }

        public void Remove(string key)
        {
            string cacheKey = string.Concat(CacheKeyPrefix, ":Cache:", key);
            Database.KeyDelete(cacheKey);
        }

        public void RemoveByPattern(string pattern)
        {
            string cacheKeyPattern = string.Concat(CacheKeyPrefix, ":Cache:", pattern);
            Database.KeyDeleteByPattern(cacheKeyPattern);
        }

        public void Clear()
        {
            string cacheKeyPattern = string.Concat(CacheKeyPrefix, ":Cache:*");
            Database.KeyDeleteByPattern(cacheKeyPattern);

            //var endPoints = connection.GetEndPoints();

            //foreach (var endPoint in endPoints)
            //{
            //    connection.GetServer(endPoint).FlushDatabase();
            //}
        }

        protected ConnectionMultiplexer GetConnection()
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException("connectionString", "Ensure 'RedisCacheConnection' is set in <appSettings>.");
            }

            return ConnectionMultiplexer.Connect(connectionString);
        }
    }
}