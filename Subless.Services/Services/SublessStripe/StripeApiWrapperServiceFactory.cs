using System;
using System.Threading;
using Microsoft.Extensions.Options;
using Subless.Models;

namespace Subless.Services.Services.SublessStripe;

public interface IStripeApiWrapperServiceFactory
{
    IStripeApiWrapperService Get();
}

public sealed class StripeApiWrapperServiceFactory : IStripeApiWrapperServiceFactory, IDisposable
{
    private const int MaxCount = 10;
    private static SemaphoreSlim _pool = new(0, MaxCount);

    public static IOptions<StripeConfig> StripeConfig { get; set; }

    public StripeApiWrapperServiceFactory()
    {
        _pool.Wait();
    }

    public IStripeApiWrapperService Get()
    {
        return new StripeApiWrapperService(StripeConfig);
    }

    public void Dispose()
    {
        _pool.Release();
    }
}
