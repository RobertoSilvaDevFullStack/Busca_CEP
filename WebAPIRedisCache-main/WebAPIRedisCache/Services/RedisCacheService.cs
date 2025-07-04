using Newtonsoft.Json;
using StackExchange.Redis;
using WebAPIRedisCache.Interfaces;

namespace WebAPIRedisCache.Services;

public class RedisCacheService : ICacheService
{
    private readonly IDatabase _cache;
    private readonly ConnectionMultiplexer _redisConnection;

    public RedisCacheService(IDatabase cache, ConnectionMultiplexer redisConnection)
    {
        _cache = cache;
        _redisConnection = redisConnection;
    }
    
    public async Task<T?> GetAsync<T>(string key)
    {
        var value = await _cache.StringGetAsync(key);
        if (value.IsNullOrEmpty)
        {
            return default(T);
        }
        return JsonConvert.DeserializeObject<T>(value!); 
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var jsonValue = JsonConvert.SerializeObject(value);
        await _cache.StringSetAsync(key, jsonValue, expiry);
    }
}