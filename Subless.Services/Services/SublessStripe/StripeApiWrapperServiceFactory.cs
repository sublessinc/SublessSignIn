using System;
using System.Threading;
using Microsoft.Extensions.Options;
using Subless.Models;

namespace Subless.Services.Services.SublessStripe;

public sealed class StripeApiWrapperServiceFactory : IStripeApiWrapperServiceFactory, IDisposable
{
    private const int MaxCount = 10;
    private static SemaphoreSlim _pool = new(MaxCount);

    public static IOptions<StripeConfig> StripeConfig { get; set; }

    public StripeApiWrapperServiceFactory()
    {
        Console.WriteLine("Requesting...");
        _pool.Wait();
        Console.WriteLine("Got one. Current count: " + (MaxCount - _pool.CurrentCount));
    }

    public IStripeApiWrapperService Get()
    {
        return new StripeApiWrapperService(StripeConfig);
    }

    public void Dispose()
    {
        _pool.Release();
        Console.WriteLine("Released. Current count: " + (MaxCount - _pool.CurrentCount));
    }
}
