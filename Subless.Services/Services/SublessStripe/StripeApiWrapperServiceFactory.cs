using System;
using System.Threading;
using Microsoft.Extensions.Options;
using Subless.Models;

namespace Subless.Services.Services.SublessStripe;

public sealed class StripeApiWrapperServiceFactory : IStripeApiWrapperServiceFactory, IDisposable
{
    public static int MaxCount { get; set; }

    private static SemaphoreSlim _pool;
    // ms: ideally not public but made so for unit testing purposes to confirm the behavior
    public static SemaphoreSlim Pool
    {
        get
        {
            return _pool ??= new SemaphoreSlim(MaxCount);
        }
    }

    public static IOptions<StripeConfig> StripeConfig { get; set; }

    public StripeApiWrapperServiceFactory()
    {
        Pool.Wait();
    }

    public IStripeApiWrapperService Get()
    {
        return new StripeApiWrapperService(StripeConfig);
    }

    public void Dispose()
    {
        Pool.Release();
    }
}
