using System;

namespace Mantle.Caching
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class CacheExtensions
    {
        public static T Get<T>(this ICacheManager cacheManager, string key, Func<T> acquire)
        {
            return Get(cacheManager, key, 60, acquire);
        }

        public static T Get<T>(this ICacheManager cacheManager, string key, int cacheTimeInMinutes, Func<T> acquire)
        {
            // TODO: Consider removing "IsSet()" and just using GetOrCreate(), because IMemoryCache does not support checking if a key is present
            //  and we are trying to keep track of the keys ourselves (See MemoryCacheManager implementation). That might lead to some problems.

            if (cacheManager.IsSet(key))
            {
                return cacheManager.Get<T>(key);
            }
            else
            {
                var result = acquire();
                //if (result != null)
                cacheManager.Set(key, result, cacheTimeInMinutes);
                return result;
            }
        }
    }
}