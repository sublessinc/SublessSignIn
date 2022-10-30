using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Subless.Models;
using Subless.Services.Services.SublessStripe;
using Xunit;

namespace Subless.Tests;

public class StripeApiWrapperServiceFactory_Tests
{
    [Fact]
    public void Factory_OnlyAllows_MaxCountInstances()
    {
        var options = Options.Create(new Models.StripeConfig());
        options.Value.SecretKey = "anything";
        options.Value.MaxInstanceCount = 20;
        
        var factory = new SingleInstanceStripeApiWrapperServiceFactory(options);
        StripeApiWrapperServiceFactory.MaxCount = 20;
        
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
            while (tasks.Any(x => x.Status == TaskStatus.WaitingToRun))
            {
                Thread.Sleep(500);
            }
            return (cancellationToken, tasks);
        }

        // Note: (SemaphoreSlim)Pool.CurrentCount reflects the total number of available slots, such that no allocations results in 
        // value returned reflecting the max available resources.
        // No instances - count is max (no allocations)
        Assert.Equal(StripeApiWrapperServiceFactory.MaxCount, SingleInstanceStripeApiWrapperServiceFactory.GetCurrentCount());
        

        // Allocate 10 - count is 0 (empty)
        var (firstBatchToken, firstBatchTasks) = CreateNInstancesAndReturnToken(StripeApiWrapperServiceFactory.MaxCount);

        Assert.Equal(0, SingleInstanceStripeApiWrapperServiceFactory.GetCurrentCount());

        // Allocate 10 - count is 0 (empty) - nothing to allocate
        var (secondBatchToken, secondBatchTasks) = CreateNInstancesAndReturnToken(StripeApiWrapperServiceFactory.MaxCount);
        Thread.Sleep(500);
        Assert.Equal(0, SingleInstanceStripeApiWrapperServiceFactory.GetCurrentCount());

        // Cancel all tasks so that all resources free up - count is max (no allocations)
        firstBatchToken.Cancel();
        secondBatchToken.Cancel();
        Thread.Sleep(500);
        Assert.Equal(StripeApiWrapperServiceFactory.MaxCount, SingleInstanceStripeApiWrapperServiceFactory.GetCurrentCount());
        
        Assert.True(firstBatchTasks.All(t => t.IsCompleted));
        Assert.True(secondBatchTasks.All(t => t.IsCompleted));
        
        // Allocate 1 - count is 9 (1 allocation)
        var (thirdBatch, thirdBatchTasks) = CreateNInstancesAndReturnToken(1);
        Thread.Sleep(500);
        Assert.Equal(StripeApiWrapperServiceFactory.MaxCount - 1, SingleInstanceStripeApiWrapperServiceFactory.GetCurrentCount());
        
        // Cancel the 1 task
        thirdBatch.Cancel();
        Thread.Sleep(500);
        Assert.Equal(StripeApiWrapperServiceFactory.MaxCount, SingleInstanceStripeApiWrapperServiceFactory.GetCurrentCount());
        Assert.True(thirdBatchTasks.All(t => t.IsCompleted));
    }

    [Fact]
    public void Factory_WithExceptionsInAction_ReleasesAndSurvives()
    {
        var options = Options.Create(new Models.StripeConfig());
        options.Value.SecretKey = "anything";
        options.Value.MaxInstanceCount = 2;
        
        var factory = new SingleInstanceStripeApiWrapperServiceFactory(options);
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
                        throw new Exception();
                    });
                    
                }, cancellationToken.Token);
                tasks.Add(task);
            }

            return (cancellationToken, tasks);
        }
        
        Assert.Equal(StripeApiWrapperServiceFactory.MaxCount, SingleInstanceStripeApiWrapperServiceFactory.GetCurrentCount());
        var (token, tasks) = CreateNInstancesAndReturnToken(StripeApiWrapperServiceFactory.MaxCount);
        Thread.Sleep(1000);
        Assert.Equal(StripeApiWrapperServiceFactory.MaxCount, SingleInstanceStripeApiWrapperServiceFactory.GetCurrentCount());
        Assert.True(tasks.All(t => t.IsCompleted));
    }

    /// <summary>
    /// Test instance of StripeApiWrapperServiceFactory with a discrete pool not affected by other tests running in parallel
    /// </summary>
    private class SingleInstanceStripeApiWrapperServiceFactory : StripeApiWrapperServiceFactory
    {
        public SingleInstanceStripeApiWrapperServiceFactory(IOptions<StripeConfig> stripeConfig)
            : base(stripeConfig)
        {
            Pool = new SemaphoreSlim(stripeConfig.Value.MaxInstanceCount);
        }

        public static int GetCurrentCount()
        {
            return Pool.CurrentCount;
        }
    }
}
