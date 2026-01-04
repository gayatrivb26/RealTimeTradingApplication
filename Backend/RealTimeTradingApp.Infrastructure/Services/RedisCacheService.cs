using Microsoft.Extensions.Caching.Distributed;
using RealTimeTradingApp.Application.Interfaces;
using System.Text.Json;

namespace RealTimeTradingApp.Infrastructure.Services
{
    public class RedisCacheService: ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

        public RedisCacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            var str = await _cache.GetStringAsync(key, cancellationToken);
            if (string.IsNullOrEmpty(str)) return default;
            return JsonSerializer.Deserialize<T>(str, _jsonOptions);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? absoluteTtl = null, CancellationToken cancellationToken = default)
        {
            var opts = new DistributedCacheEntryOptions();
            if (absoluteTtl.HasValue)
                opts.AbsoluteExpirationRelativeToNow = absoluteTtl;

            var str = JsonSerializer.Serialize(value, _jsonOptions);
            await _cache.SetStringAsync(key, str, opts, cancellationToken);
        }

        public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            return _cache.RemoveAsync(key, cancellationToken);
        }


    }
}
