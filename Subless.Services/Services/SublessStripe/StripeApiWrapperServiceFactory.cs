using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Subless.Models;

namespace Subless.Services.Services.SublessStripe;

public sealed class StripeApiWrapperServiceFactory : IStripeApiWrapperServiceFactory
{
    private int _timeoutSeconds = 10 * 1000;
    private static SemaphoreSlim _pool;
    public static SemaphoreSlim Pool
    {
        get
        {
            return _pool ??= new SemaphoreSlim(MaxCount);
        }
    }

    public static int MaxCount { get; set; }

    private readonly IOptions<StripeConfig> _stripeConfig;

    public StripeApiWrapperServiceFactory(IOptions<StripeConfig> stripeConfig)
    {
        _stripeConfig = stripeConfig;
        MaxCount = stripeConfig.Value.MaxInstanceCount;
    }

    public void Execute(Action<IStripeApiWrapperService> action)
    {
        try
        {
            Pool.Wait(_timeoutSeconds);
            action(new StripeApiWrapperService(_stripeConfig));
        }
        finally
        {
            Pool.Release();
        }
    }

    public T Execute<T>(Func<IStripeApiWrapperService, T> action)
    {
        try
        {
            Pool.Wait(_timeoutSeconds);
            return action(new StripeApiWrapperService(_stripeConfig));
        }
        finally
        {
            Pool.Release();
        }
    }

    public async Task ExecuteAsync(Func<IStripeApiWrapperService, Task> action)
    {
        try
        {
            await Pool.WaitAsync(_timeoutSeconds);
            await action(new StripeApiWrapperService(_stripeConfig));
        }
        finally
        {
            Pool.Release();
        }
    }

    public async Task<T> ExecuteAsync<T>(Func<IStripeApiWrapperService, Task<T>> action)
    {
        try
        {
            await Pool.WaitAsync(_timeoutSeconds);
            return await action(new StripeApiWrapperService(_stripeConfig));
        }
        finally
        {
            Pool.Release();
        }
    }
}
