using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Subless.Models;

namespace Subless.Services.Services.SublessStripe;

public interface IStripeApiWrapperServiceFactory
{
    Task<IStripeApiWrapperService> GetAsync();
    void Release();
}

public sealed class StripeApiWrapperServiceFactory : IStripeApiWrapperServiceFactory
{
    private const int MaxCount = 10;
    private static SemaphoreSlim _pool = new(0, MaxCount);

    public static IOptions<StripeConfig> StripeConfig { get; set; }

    public async Task<IStripeApiWrapperService> GetAsync()
    {
        await _pool.WaitAsync();
        return new StripeApiWrapperService(StripeConfig);
    }

    public void Release()
    {
        _pool.Release();
    }
}
