using Microsoft.Extensions.Caching.Memory;

namespace Subless.Services.Services
{
    public interface ICacheService
    {
        IMemoryCache Cache { get; }

        void InvalidateCache();
    }
}