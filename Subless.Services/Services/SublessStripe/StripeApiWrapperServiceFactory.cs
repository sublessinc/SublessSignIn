using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Subless.Models;

namespace Subless.Services.Services.SublessStripe;

public sealed class StripeApiWrapperServiceFactory : IStripeApiWrapperServiceFactory
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

    private readonly IOptions<StripeConfig> _stripeConfig;

    public StripeApiWrapperServiceFactory(IOptions<StripeConfig> stripeConfig)
    {
        _stripeConfig = stripeConfig;
    }

    public void Execute(Action<IStripeApiWrapperService> action)
    {
        Pool.Wait();
        action(new StripeApiWrapperService(_stripeConfig));
        Pool.Release();
    }

    public T Execute<T>(Func<IStripeApiWrapperService, T> action)
    {
        Pool.Wait();
        var result = action(new StripeApiWrapperService(_stripeConfig));
        Pool.Release();
        return result;
    }

    public async Task ExecuteAsync(Func<IStripeApiWrapperService, Task> action)
    {
        await Pool.WaitAsync();
        await action(new StripeApiWrapperService(_stripeConfig));
        Pool.Release();
    }

    public async Task<T> ExecuteAsync<T>(Func<IStripeApiWrapperService, Task<T>> action)
    {
        await Pool.WaitAsync();
        var result = await action(new StripeApiWrapperService(_stripeConfig));
        Pool.Release();
        return result;
    }
}
