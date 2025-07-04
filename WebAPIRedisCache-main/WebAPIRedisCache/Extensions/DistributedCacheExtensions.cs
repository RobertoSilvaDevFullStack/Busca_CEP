using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace ProjetoCep.Api.Extensions
{
    public static class DistributedCacheExtensions
    {
        public static async Task<T?> GetAsync<T>(this IDistributedCache cache, string key)
        {
            var json = await cache.GetStringAsync(key);
            if (json == null)
            {
                return default;
            }
            return JsonSerializer.Deserialize<T>(json);
        }

        public static async Task SetAsync<T>(this IDistributedCache cache, string key, T value, int? absoluteExpirationRelativeToNowMinutes = null)
        {
            var options = new DistributedCacheEntryOptions();
            if (absoluteExpirationRelativeToNowMinutes.HasValue)
            {
                options.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(absoluteExpirationRelativeToNowMinutes.Value);
            }
            else
            {
                options.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
            }

            var json = JsonSerializer.Serialize(value);
            await cache.SetStringAsync(key, json, options);
        }
    }
}