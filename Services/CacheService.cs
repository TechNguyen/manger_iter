using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using NRedisStack.RedisStackCommands;
using StackExchange.Redis;

namespace It_Supporter.Services
{
    public class CacheService : ICacheService
    {
        private IDatabase _cachedatabase;
        public CacheService() {
            var redis = ConnectionMultiplexer.Connect("localhost:6379");
            _cachedatabase = redis.GetDatabase();
            var ft = _cachedatabase.FT();
            var json = _cachedatabase.JSON();
        }
        public T GetData<T>(string key)
        {
            var value = _cachedatabase.StringGet(key);
            if(!string.IsNullOrEmpty(value)) {
                return JsonSerializer.Deserialize<T>(value);
            }
            return default; 
        }

        public async Task<bool> RefreshData(string key)
        {
            TimeSpan newExpiration  = TimeSpan.FromMinutes(3);
            var value = _cachedatabase.StringGet(key);
            if(!string.IsNullOrEmpty(value)) {
                return await _cachedatabase.KeyExpireAsync(key, newExpiration);
            }
            return false;
        }
        public object RemoveData(string key)
        {
            var _exits = _cachedatabase.KeyExists(key);
            if(_exits)
                return _cachedatabase.KeyDelete(key);
            return false;
        }

        public bool SetData<T>(string key, T value, DateTimeOffset expriseTime)
        {
            var exprirationTime = expriseTime.DateTime.Subtract(DateTime.Now);
            var isSet = _cachedatabase.StringSet(key, JsonSerializer.Serialize(value), exprirationTime);
            return isSet;
        }
    }
}