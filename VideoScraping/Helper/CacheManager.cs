using Microsoft.Extensions.Caching.Memory;

namespace VideoScraping.Helper;

public class CacheManager
{
    private readonly IMemoryCache _memoryCache;

    public CacheManager(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public T GetOrCreate<T>(string key, Func<T> factory, TimeSpan? expiration = null)
    {
        if (_memoryCache.TryGetValue(key, out T value))
        {
            return value;
        }

        value = factory();
        if (value == null)
        {
            return value;
        }

        _memoryCache.Set(key, value, expiration ?? TimeSpan.FromDays(1));
        return value;
    }

    public void Remove(string key)
    {
        _memoryCache.Remove(key);
    }
}