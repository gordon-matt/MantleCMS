using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Mantle.Caching
{
    /// <summary>
    /// Represents a MemoryCacheCache
    /// </summary>
    public partial class MemoryCacheManager : ICacheManager
    {
        private readonly HashSet<string> keys;
        private readonly IServiceProvider serviceProvider;

        public MemoryCacheManager(IServiceProvider serviceProvider)
        {
            this.keys = new HashSet<string>();
            this.serviceProvider = serviceProvider;
        }

        protected IMemoryCache Cache
        {
            get { return serviceProvider.GetService<IMemoryCache>(); }
        }

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">The key of the value to get.</param>
        /// <returns>The value associated with the specified key.</returns>
        public virtual T Get<T>(string key)
        {
            return Cache.Get<T>(key);
        }

        /// <summary>
        /// Adds the specified key and object to the cache.
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="data">Data</param>
        /// <param name="cacheTimeInMinutes">Cache time</param>
        public virtual void Set(string key, object data, int cacheTimeInMinutes)
        {
            if (data == null)
            {
                return;
            }

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(cacheTimeInMinutes));

            Cache.Set(key, data, cacheEntryOptions);

            if (!keys.Contains(key))
            {
                keys.Add(key);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the value associated with the specified key is cached
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>Result</returns>
        public virtual bool IsSet(string key)
        {
            return keys.Contains(key);

            //object cacheEntry;
            //return Cache.TryGetValue(key, out cacheEntry);
        }

        /// <summary>
        /// Removes the value with the specified key from the cache
        /// </summary>
        /// <param name="key">/key</param>
        public virtual void Remove(string key)
        {
            Cache.Remove(key);

            if (keys.Contains(key))
            {
                keys.Remove(key);
            }
        }

        /// <summary>
        /// Removes items by pattern
        /// </summary>
        /// <param name="pattern">pattern</param>
        public virtual void RemoveByPattern(string pattern)
        {
            var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var keysToRemove = new List<String>();

            foreach (string key in keys)
            {
                if (regex.IsMatch(key))
                {
                    keysToRemove.Add(key);
                }
            }

            foreach (string key in keysToRemove)
            {
                Remove(key);
            }
        }

        /// <summary>
        /// Clear all cache data
        /// </summary>
        public virtual void Clear()
        {
            foreach (string key in keys)
            {
                Remove(key);
            }
        }
    }
}