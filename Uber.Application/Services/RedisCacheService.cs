using StackExchange.Redis;
using System.Text.Json;
using Uber.Uber.Application.Interfaces;

namespace Uber.Uber.Application.Services
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDatabase _db;

        public RedisCacheService(IConnectionMultiplexer redis  )
        {
            _db = redis.GetDatabase();

        }

        public T Get<T>(string key)
        {
           var Value = _db.StringGet(key);
            if (Value.IsNullOrEmpty) return default;
            return JsonSerializer.Deserialize<T>(Value!);
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var value = await _db.StringGetAsync(key);
            if (value.IsNullOrEmpty) return default;
            return JsonSerializer.Deserialize<T>(value!);
        }

        public void Remove(string key)
        {
           _db.KeyDelete(key);
        }

        public async Task RemoveAsync(string key)
        {
            await _db.KeyDeleteAsync(key);

        }

        public void Set<T>(string key, T value, TimeSpan duration)
        {
             var data = JsonSerializer.Serialize(value);
            _db.StringSet(key, data, duration);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan duration)
        {
            var data = JsonSerializer.Serialize(value);
            await _db.StringSetAsync(key, data, duration);
        }
    }
}
