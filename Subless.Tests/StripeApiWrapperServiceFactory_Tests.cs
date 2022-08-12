using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Subless.Services.Services.SublessStripe;
using Xunit;

namespace Subless.Tests;

public class StripeApiWrapperServiceFactory_Tests
{
    [Fact]
    public void Factory_WithExceptionsInAction_ReleasesAndSurvives()
    {
        
    }
    
    [Fact]
    public void Factory_OnlyAllows_MaxCountInstances()
    {
        var options = Options.Create(new Models.StripeConfig());
        options.Value.SecretKey = "anything";
        var factory = new StripeApiWrapperServiceFactory(options);
        StripeApiWrapperServiceFactory.MaxCount = 10;
        
        (CancellationTokenSource, List<Task>) CreateNInstancesAndReturnToken(int count)
        {
            var tasks = new List<Task>();
            var cancellationToken = new CancellationTokenSource();
            for (var i = 0; i < count; i++)
            {
                var task = Task.Run(() =>
                {
                    factory.ExecuteAsync(async api =>
                    {
                        WaitHandle.WaitAny(new[] {cancellationToken.Token.WaitHandle});
                    });
                    
                }, cancellationToken.Token);
                tasks.Add(task);
            }

            return (cancellationToken, tasks);
        }

        // Note: (SemaphoreSlim)Pool.CurrentCount reflects the total number of available slots, such that no allocations results in 
        // value returned reflecting the max available resources.
        // No instances - count is max (no allocations)
        Assert.Equal(StripeApiWrapperServiceFactory.MaxCount, StripeApiWrapperServiceFactory.Pool.CurrentCount);
        
        // Allocate 10 - count is 0 (empty)
        var (firstBatchToken, firstBatchTasks) = CreateNInstancesAndReturnToken(StripeApiWrapperServiceFactory.MaxCount);
        Thread.Sleep(500);
        Assert.Equal(0, StripeApiWrapperServiceFactory.Pool.CurrentCount);

        // Allocate 10 - count is 0 (empty) - nothing to allocate
        var (secondBatchToken, secondBatchTasks) = CreateNInstancesAndReturnToken(StripeApiWrapperServiceFactory.MaxCount);
        Thread.Sleep(500);
        Assert.Equal(0, StripeApiWrapperServiceFactory.Pool.CurrentCount);

        // Cancel all tasks so that all resources free up - count is max (no allocations)
        firstBatchToken.Cancel();
        secondBatchToken.Cancel();
        Thread.Sleep(500);
        Assert.Equal(StripeApiWrapperServiceFactory.MaxCount, StripeApiWrapperServiceFactory.Pool.CurrentCount);
        
        Assert.True(firstBatchTasks.All(t => t.IsCompleted));
        Assert.True(secondBatchTasks.All(t => t.IsCompleted));
        
        // Allocate 1 - count is 9 (1 allocation)
        var (thirdBatch, thirdBatchTasks) = CreateNInstancesAndReturnToken(1);
        Thread.Sleep(500);
        Assert.Equal(StripeApiWrapperServiceFactory.MaxCount - 1, StripeApiWrapperServiceFactory.Pool.CurrentCount);
        
        // Cancel the 1 task
        thirdBatch.Cancel();
        Thread.Sleep(500);
        Assert.Equal(StripeApiWrapperServiceFactory.MaxCount, StripeApiWrapperServiceFactory.Pool.CurrentCount);
        Assert.True(thirdBatchTasks.All(t => t.IsCompleted));
    }
}
