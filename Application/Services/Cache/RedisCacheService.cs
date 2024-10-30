using Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
namespace Application.Services.Cache
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;
        public RedisCacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }
        public async Task<T?> GetCachedDataAsync<T>(string cacheKey)
        {
            var cachedData = await _distributedCache.GetStringAsync(cacheKey);
            if (cachedData == null) return default;

            return JsonSerializer.Deserialize<T>(cachedData);
        }

        public async Task SetCachedDataAsync<T>(string cacheKey, T data)
        {
            var serializedData = JsonSerializer.Serialize(data);
            var expirationTime = TimeSpan.FromMinutes(10) + TimeSpan.FromSeconds(new Random().Next(0, 60));

            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expirationTime
            };

            await _distributedCache.SetStringAsync(cacheKey, serializedData, cacheOptions);
        }

        public async Task RemoveCachedDataAsync(string cacheKey)
        {
            await _distributedCache.RemoveAsync(cacheKey);
        }

        public async Task SetCachedDataAsyncWithTime<T>(string cacheKey, T data, TimeSpan expirationTime)
        {
            var serializedData = JsonSerializer.Serialize(data);
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expirationTime
            };
            await _distributedCache.SetStringAsync(cacheKey,serializedData,cacheOptions);
        }
    }
}
