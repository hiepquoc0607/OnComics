using Microsoft.Extensions.Caching.Distributed;
using OnComics.Application.Services.Interfaces;
using System.Text.Json;

namespace OnComics.Application.Services.Implements
{
    public class RedisService : IRedisService
    {
        private readonly IDistributedCache _cache;
        private readonly JsonSerializerOptions _jsonOptions;

        public RedisService(IDistributedCache cache)
        {
            _cache = cache;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        //Get Cache Data From Redis
        public async Task<T?> GetAsync<T>(string key)
        {
            var bytes = await _cache.GetAsync(key, default);

            if (bytes == null || bytes.Length == 0)
                return default;

            return JsonSerializer.Deserialize<T>(bytes, _jsonOptions);
        }

        //Set Cache Data To Redis
        public async Task SetAsync<T>(
            string key,
            T value,
            TimeSpan? absoluteExpiration = null)
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes(value, _jsonOptions);
            var options = new DistributedCacheEntryOptions();

            if (absoluteExpiration.HasValue)
            {
                options.SetAbsoluteExpiration(absoluteExpiration.Value);
            }
            else
            {
                // default 5 minutes
                options.SetSlidingExpiration(TimeSpan.FromMinutes(5));
            }

            await _cache.SetAsync(key, bytes, options, default);
        }

        //Remove Cache Data From Redis
        public async Task RemoveAsync(string key)
        {
            await _cache.RemoveAsync(key, default);
        }
    }
}
