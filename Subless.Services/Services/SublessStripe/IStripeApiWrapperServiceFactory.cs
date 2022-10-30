using System;
using System.Threading.Tasks;

namespace Subless.Services.Services.SublessStripe;

public interface IStripeApiWrapperServiceFactory
{
    void Execute(Action<IStripeApiWrapperService> action);
    T Execute<T>(Func<IStripeApiWrapperService, T> action);
    Task ExecuteAsync(Func<IStripeApiWrapperService, Task> action);
    Task<T> ExecuteAsync<T>(Func<IStripeApiWrapperService, Task<T>> action);
}
