using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Stripe;
using Subless.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Subless.Tests
{
    public class StripeService_Tests
    {
        [Fact]
        public void GetPayers_Executes()
        {
            var sut = StripeServiceBuilder.BuildStripeService();
            sut.GetPayersForRange(new DateTimeOffset(), new DateTimeOffset());
        }

        public static class StripeServiceBuilder 
        {
            public static StripeService BuildStripeService()
            {
                var serviceProvider = new ServiceCollection()
                    .AddLogging(x =>
                    {
                        x.AddSimpleConsole();
                    })
                    .BuildServiceProvider();
                var factory = serviceProvider.GetService<ILoggerFactory>();
                var stripeWrapper = new Mock<IStripeApiWrapperService>();
                var invoiceService = new Mock<InvoiceService>();
                invoiceService.Setup(x => x.List(It.IsAny<InvoiceListOptions>(), null)).Returns(new StripeList<Invoice>());
                stripeWrapper.Setup(x=>x.InvoiceService).Returns(invoiceService.Object);
                var sut = new StripeService(Options.Create(new Models.StripeConfig()), Mock.Of<IUserService>(), stripeWrapper.Object, factory);
                return sut;
            }
        }
    }
}
