using System.Security.Cryptography.X509Certificates;
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

            ConfigurationOptions options = new ConfigurationOptions {
                EndPoints = {{"redis-14916.c1.ap-southeast-1-1.ec2.cloud.redislabs.com", 14916}},
                User = "default",
                Password = "esEi9Y9Hiuy9GO6O7duIJkqH4mWqTy3t",
                // Ssl = true,
                // SslProtocols = System.Security.Authentication.SslProtocols.Tls12,
            };


           
            // options.CertificateSelection += delegate
            // {
            //     return new X509Certificate2("redis.pfx", "esEi9Y9Hiuy9GO6O7duIJkqH4mWqTy3t"); // use the password you specified for pfx file
            // };
            // options += ValidateServerCertificate;

            // bool ValidateServerCertificate(
            //         object sender,
            //         X509Certificate? certificate,
            //         X509Chain? chain,
            //         SslPolicyErrors sslPolicyErrors)
            // {
            //     if (certificate == null) {
            //         return false;       
            //     }

            //     var ca = new X509Certificate2("redis_ca.pem");
            //     bool verdict = (certificate.Issuer == ca.Subject);
            //     if (verdict) {
            //         return true;
            //     }
            //     Console.WriteLine("Certificate error: {0}", sslPolicyErrors);
            //     return false;
            // }

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