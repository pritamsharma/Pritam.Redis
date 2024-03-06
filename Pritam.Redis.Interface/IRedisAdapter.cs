

namespace Pritam.Redis.Interface
{
    public interface IRedisAdapter
    {
        Task<bool> Set<T>(string key, T value);

        Task<T?> Get<T>(string key);

        Task<bool> IsSet(string key);

        Task<bool> Remove(string key);

        Task<bool> RemoveSessionData();

        Task<bool> SetHash<T>(string key, string hashKey, T value);

        Task<T?> GetHash<T>(string key, string hashKey);

        Task<bool> RemoveHash(string key, string hashKey);

        Task<bool> IsHashSet(string key, string hashKey);
    }
}
