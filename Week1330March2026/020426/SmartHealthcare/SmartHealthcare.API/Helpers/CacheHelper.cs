using Microsoft.Extensions.Caching.Memory;

namespace SmartHealthcare.API.Helpers
{
    /// <summary>
    /// Helper service for managing cache operations
    /// </summary>
    public class CacheHelper
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<CacheHelper> _logger;
        private static readonly HashSet<string> CacheKeys = new();

        public CacheHelper(IMemoryCache cache, ILogger<CacheHelper> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        // Cache key constants
        public static class Keys
        {
            public const string AllDoctors = "all_doctors";
            public const string AllSpecializations = "all_specializations";
            public const string AllPatients = "all_patients";
            public const string Doctor = "doctor_{0}"; // {id}
            public const string Patient = "patient_{0}"; // {id}
            public const string DoctorsBySpecialization = "doctors_spec_{0}"; // {specializationId}
        }

        /// <summary>
        /// Get value from cache or set it if not exists
        /// </summary>
        public async Task<T?> GetOrSetAsync<T>(
            string key,
            Func<Task<T>> factory,
            TimeSpan? absoluteExpiration = null,
            TimeSpan? slidingExpiration = null)
        {
            if (_cache.TryGetValue(key, out T? cachedValue))
            {
                _logger.LogInformation("Cache hit for key: {Key}", key);
                return cachedValue;
            }

            _logger.LogInformation("Cache miss for key: {Key}, fetching from source", key);
            var value = await factory();

            if (value != null)
            {
                var cacheOptions = new MemoryCacheEntryOptions();

                if (absoluteExpiration.HasValue)
                    cacheOptions.AbsoluteExpirationRelativeToNow = absoluteExpiration;

                if (slidingExpiration.HasValue)
                    cacheOptions.SlidingExpiration = slidingExpiration;
                else
                    cacheOptions.SlidingExpiration = TimeSpan.FromMinutes(30); // Default: 30 min

                _cache.Set(key, value, cacheOptions);
                CacheKeys.Add(key);
            }

            return value;
        }

        /// <summary>
        /// Remove specific cache entry
        /// </summary>
        public void Remove(string key)
        {
            _cache.Remove(key);
            CacheKeys.Remove(key);
            _logger.LogInformation("Cache removed for key: {Key}", key);
        }

        /// <summary>
        /// Remove multiple cache entries by pattern
        /// </summary>
        public void RemoveByPattern(string pattern)
        {
            var keysToRemove = CacheKeys.Where(k => k.Contains(pattern)).ToList();
            foreach (var key in keysToRemove)
            {
                Remove(key);
            }
            _logger.LogInformation("Removed {Count} cache entries matching pattern: {Pattern}", 
                keysToRemove.Count, pattern);
        }

        /// <summary>
        /// Clear all cache
        /// </summary>
        public void ClearAll()
        {
            foreach (var key in CacheKeys.ToList())
            {
                _cache.Remove(key);
            }
            CacheKeys.Clear();
            _logger.LogInformation("All cache cleared");
        }

        /// <summary>
        /// Set value in cache with default expiration
        /// </summary>
        public void Set<T>(string key, T value, TimeSpan? expiration = null)
        {
            var cacheOptions = new MemoryCacheEntryOptions
            {
                SlidingExpiration = expiration ?? TimeSpan.FromMinutes(30)
            };

            _cache.Set(key, value, cacheOptions);
            CacheKeys.Add(key);
            _logger.LogInformation("Cache set for key: {Key}", key);
        }
    }
}
