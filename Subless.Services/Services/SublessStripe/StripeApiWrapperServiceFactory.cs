using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Subless.Models;

namespace Subless.Services.Services.SublessStripe;

public class StripeApiWrapperServiceFactory : IStripeApiWrapperServiceFactory
{
    private const int TimeoutSeconds = 10 * 1000;
    private static SemaphoreSlim _pool;
    protected static SemaphoreSlim Pool
    {
        get
        {
            return _pool ??= new SemaphoreSlim(MaxCount);
        }
        set => _pool = value;
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
            Pool.Wait(TimeoutSeconds);
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
            Pool.Wait(TimeoutSeconds);
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
            await Pool.WaitAsync(TimeoutSeconds);
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
            await Pool.WaitAsync(TimeoutSeconds);
            return await action(new StripeApiWrapperService(_stripeConfig));
        }
        finally
        {
            Pool.Release();
        }
    }
}
