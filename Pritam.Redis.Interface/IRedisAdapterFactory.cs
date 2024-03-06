
namespace Pritam.Redis.Interface
{
    public interface IRedisAdapterFactory: IDisposable
    {
        IRedisAdapter CreateCacheAdapter();
    }
}
