using Microsoft.Extensions.Caching.Memory;
using MemoryCache = Microsoft.Extensions.Caching.Memory.MemoryCache;

namespace Subless.Services.Services
{
    public class CacheService : ICacheService
    {
        private IMemoryCache _cache;
        public CacheService(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        public IMemoryCache Cache => _cache;

        public void InvalidateCache()
        {
            ((MemoryCache)_cache).Compact(0.0);
        }
    }
}
