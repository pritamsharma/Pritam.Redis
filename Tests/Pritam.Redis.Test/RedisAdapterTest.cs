using Pritam.Redis.Interface;
using Pritam.Redis;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace InMemory.Cache.Test
{
    [TestClass]
    public class RedisAdapterTest
    {

        IRedisAdapter CacheAdapter { get; set; }

        public RedisAdapterTest()
        {
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            var configuration = builder.Build();

            string connectionString = configuration["AppSettings:RedisCache"] ?? string.Empty;
            var expiryTimeSeconds = Convert.ToInt32(configuration["AppSettings:ExpiryTime"]);
            string keyPrefix = configuration["AppSettings:RedisCacheKeyPrefix"] ?? string.Empty;
            var sessionId = new Random().Next().ToString();

            var redisAdapterFactory = new RedisAdapterFactory(connectionString ?? string.Empty, expiryTimeSeconds, sessionId, keyPrefix ?? string.Empty);

            CacheAdapter = redisAdapterFactory.CreateCacheAdapter();
        }

        private JObject GenerateTestData
        {
            get
            {
                var randomGenerater = new Random();

                dynamic testData = new JObject();
                testData.AgencyId = randomGenerater.Next();
                testData.EntityId = randomGenerater.Next();
                testData.EntityType = Guid.NewGuid().ToString();
                testData.ConsentValue = Guid.NewGuid().ToString();
                testData.TenantId = randomGenerater.Next();
                testData.SalesPersonId = randomGenerater.Next();
                testData.Success = randomGenerater.Next() % 2 == 0 ? "Yes" : "No";
                testData.CreatedBy = randomGenerater.Next();
                testData.CreatedDate = DateTime.Now.ToShortDateString();

                return testData;
            }
        }

        [TestMethod]
        public async Task SetTestPositive()
        {
            var key = "SetTestPositive_Key";

            var isSuccess = await SetValue(key);

            _ = await RemoveKey(key);

            Assert.IsTrue(isSuccess);
        }

        private async Task<bool> SetValue(string key)
        {
            return await CacheAdapter.Set(key, GenerateTestData);
        }

        [TestMethod]
        public async Task GetTestPositive()
        {
            var key = "GetTestPositive_Key";

            _ = await SetValue(key);

            var cacheValue = await CacheAdapter.Get<JObject>(key);

            _ = await RemoveKey(key);

            Assert.IsTrue(cacheValue != null && cacheValue.HasValues);
        }

        [TestMethod]
        public async Task GetTestNegative()
        {
            var key = "GetTestNegative_Key";

            var cacheValue = await CacheAdapter.Get<JObject>(key);

            Assert.IsTrue(cacheValue == null || !cacheValue.HasValues);
        }

        [TestMethod]
        public async Task RemoveTestPositive()
        {
            var key = "RemoveTestPositive_Key";

            _ = await SetValue(key);

            var isSuccess = await RemoveKey(key);

            Assert.IsTrue(isSuccess);
        }

        [TestMethod]
        public async Task RemoveTestNegative()
        {
            var key = "RemoveTestNegative_Key";

            var isSuccess = await RemoveKey(key);

            Assert.IsFalse(isSuccess);
        }

        private async Task<bool> RemoveKey(string key)
        {
            return await CacheAdapter.Remove(key);
        }

        [TestMethod]
        public async Task IsSetTestPositive()
        {
            var key = "IsSetTestPositive_Key";

            _ = await SetValue(key);

            var isSuccess = await CacheAdapter.IsSet(key);

            _ = await RemoveKey(key);

            Assert.IsTrue(isSuccess);
        }

        [TestMethod]
        public async Task IsSetTestNegative()
        {
            var key = "IsSetTestNegative_Key";

            var isSuccess = await CacheAdapter.IsSet(key);

            Assert.IsFalse(isSuccess);
        }

        [TestMethod]
        public async Task Z_RemoveAllSessionData()
        {
            for (var i = 0; i < 10; i++)
            {
                var key = "Z_RemoveAllSessionData" + new Random().Next(1000, 9999);
                _ = await SetValue(key);
            }

            var isSuccess = await CacheAdapter.RemoveSessionData();

            Assert.IsTrue(isSuccess);
        }

        [TestMethod]
        public async Task SetHashTestPositive()
        {
            var key = "SetHashTestPositive_Key";
            var hashKey = "SetHashTestPositive_Value";

            var isSuccess = await SetHashValue(key, hashKey);

            _ = await RemoveHashKey(key, hashKey);

            Assert.IsTrue(isSuccess);
        }

        private async Task<bool> SetHashValue(string key, string hashKey)
        {
            return await CacheAdapter.SetHash(key, hashKey, GenerateTestData);
        }

        [TestMethod]
        public async Task GetHashTestPositive()
        {
            var key = "GetHashTestPositive_Key";
            var hashKey = "GetHashTestPositive_Hash";

            _ = await SetHashValue(key, hashKey);

            var cacheValue = await CacheAdapter.GetHash<JObject>(key, hashKey);

            _ = await RemoveHashKey(key, hashKey);

            Assert.IsTrue(cacheValue != null && cacheValue.HasValues);
        }

        [TestMethod]
        public async Task GetHashTestNegative()
        {
            var key = "GetHashTestNegative_Key";
            var hashKey = "GetHashTestNegative_Hash";

            var cacheValue = await CacheAdapter.GetHash<JObject>(key, hashKey);

            Assert.IsTrue(cacheValue == null || !cacheValue.HasValues);
        }

        [TestMethod]
        public async Task RemoveHashTestPositive()
        {
            var key = "RemoveHashTestPositive_Key";
            var hashKey = "RemoveHashTestPositive_Hash";

            _ = await SetHashValue(key, hashKey);

            var isSuccess = await RemoveHashKey(key, hashKey);

            Assert.IsTrue(isSuccess);
        }

        [TestMethod]
        public async Task RemoveHashTestNegative()
        {
            var key = "RemoveHashTestNegative_Key";
            var hashKey = "RemoveHashTestNegative_Hash";

            var isSuccess = await RemoveHashKey(key, hashKey);

            Assert.IsFalse(isSuccess);
        }

        private async Task<bool> RemoveHashKey(string key, string hashKey)
        {
            return await CacheAdapter.RemoveHash(key, hashKey);
        }

        [TestMethod]
        public async Task IsHashSetTestPositive()
        {
            var key = "IsHashSetTestPositive_Key";
            var hashKey = "IsHashSetTestPositive_Hash";

            _ = await SetHashValue(key, hashKey);

            var isSuccess = await CacheAdapter.IsHashSet(key, hashKey);

            _ = await RemoveHashKey(key, hashKey);

            Assert.IsTrue(isSuccess);
        }

        [TestMethod]
        public async Task IsHashSetTestNegative()
        {
            var key = "IsHashSetTestNegative_Key";
            var hashKey = "IsHashSetTestNegative_Hash";

            var isSuccess = await CacheAdapter.IsHashSet(key, hashKey);

            Assert.IsFalse(isSuccess);
        }

    }
}
