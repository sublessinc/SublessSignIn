using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Stripe;
using Subless.Models;
using Subless.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Subless.Tests
{
    public class StripeService_Tests
    {
        [Fact]
        public async Task GetPayers_Executes() // verifies that the mocks work to construct the object
        {
            var sut = StripeServiceBuilder.BuildStripeService();
            await sut.GetPayersForRange(new DateTimeOffset(), new DateTimeOffset());
        }

        [Fact]
        public async Task GetPayers_WithOneInvoiceAndNoUsers_ReturnsNoPayers()
        {
            var invoices = new List<Invoice>() { new Invoice() };
            var sut = StripeServiceBuilder.BuildStripeService(invoices);
            var payers = await sut.GetPayersForRange(new DateTimeOffset(), new DateTimeOffset());
            Assert.Empty(payers);
        }

        [Fact]
        public async Task GetPayers_WithOneInvoiceAndOneUser_ReturnsOnePayer()
        {
            var stripeId = "stripeId";
            var invoices = new List<Invoice>() { new Invoice() { CustomerId = stripeId } };
            var users = new List<User>() { new User() { StripeCustomerId = stripeId, Id = Guid.NewGuid() } };
            var sut = StripeServiceBuilder.BuildStripeService(invoices, users);
            var payers = await sut.GetPayersForRange(new DateTimeOffset(), new DateTimeOffset());
            Assert.Single(payers);
            Assert.Equal(users.Single().Id, payers.Single().UserId);
        }

        [Fact]
        public async Task GetPayers_WithInvoiceAndFullRefund_ReturnsNoPayment()
        {
            var stripeId = "stripeId";
            var chargeId = "chargeId";
            var invoices = new List<Invoice>() { new Invoice() { CustomerId = stripeId, ChargeId = chargeId} };
            var charge = new Charge() { Id = chargeId,  Amount = 1000 };
            var refund = new Refund() { ChargeId = chargeId, Amount = 1000 };
            var users = new List<User>() { new User() { StripeCustomerId = stripeId, Id = Guid.NewGuid() } };
            var sut = StripeServiceBuilder.BuildStripeService(
                invoices, 
                users, 
                new List<Refund> { refund} , 
                new List<Charge> { charge },
                new BalanceTransaction());
            var payers = await sut.GetPayersForRange(new DateTimeOffset(), new DateTimeOffset());
            Assert.Single(payers);
            Assert.Equal(users.Single().Id, payers.Single().UserId);
            Assert.Equal(0, payers.Single().Payment);
        }
        [Fact]
        public async Task GetPayers_WithInvoiceWithFees_ReturnsFeesInPayment()
        {
            var stripeId = "stripeId";
            var chargeId = "chargeId";
            var invoices = new List<Invoice>() { new Invoice() { CustomerId = stripeId, ChargeId = chargeId } };
            var charge = new Charge() { Id = chargeId, Amount = 1000 };
            var users = new List<User>() { new User() { StripeCustomerId = stripeId, Id = Guid.NewGuid() } };
            var sut = StripeServiceBuilder.BuildStripeService(
                invoices,
                users,
                new List<Refund> { },
                new List<Charge> { charge },
                new BalanceTransaction(){ Fee = 97 });
            var payers = await sut.GetPayersForRange(new DateTimeOffset(), new DateTimeOffset());
            Assert.Single(payers);
            Assert.Equal(users.Single().Id, payers.Single().UserId);
            Assert.Equal(97, payers.Single().Fees);
        }

        [Fact]
        public async Task GetPayers_WithInvoiceWithTaxes_ReturnsTaxesInPayment()
        {
            var stripeId = "stripeId";
            var chargeId = "chargeId";
            var invoices = new List<Invoice>() { new Invoice() { CustomerId = stripeId, ChargeId = chargeId, Tax = 22 } };
            var charge = new Charge() { Id = chargeId, Amount = 1000 };
            var users = new List<User>() { new User() { StripeCustomerId = stripeId, Id = Guid.NewGuid() } };
            var sut = StripeServiceBuilder.BuildStripeService(
                invoices,
                users,
                new List<Refund> { },
                new List<Charge> { charge },
                new BalanceTransaction() { Fee = 97 });
            var payers = await sut.GetPayersForRange(new DateTimeOffset(), new DateTimeOffset());
            Assert.Single(payers);
            Assert.Equal(users.Single().Id, payers.Single().UserId);
            Assert.Equal(22, payers.Single().Taxes);
        }

        [Fact]
        public async Task GetPayers_WithInvoiceWithMultiplePages_ReturnsAllPayments()
        {
            var stripeId = "stripeId";
            var chargeId = "chargeId";
            var invoiceService = new Mock<InvoiceService>();
            var invoices = new StripeList<Invoice>() { Data = new List<Invoice>() { new Invoice() { CustomerId = stripeId, ChargeId = chargeId } }};
            var invoices2 = new StripeList<Invoice>() { Data = new List<Invoice>() { new Invoice() { CustomerId = stripeId, ChargeId = chargeId } } };
            invoiceService.SetupSequence(x => x.ListAsync(It.IsAny<InvoiceListOptions>(), null, new CancellationToken()))
                .ReturnsAsync(invoices)
                .ReturnsAsync(invoices2)
                .ReturnsAsync(new StripeList<Invoice>() { Data = new List<Invoice>() }); 
            var charge = new Charge() { Id = chargeId, Amount = 1000 };
            var users = new List<User>() { new User() { StripeCustomerId = stripeId, Id = Guid.NewGuid() } };
            var sut = StripeServiceBuilder.BuildStripeService(
                null,
                users,
                new List<Refund> { },
                new List<Charge> { charge },
                new BalanceTransaction() { },
                invoiceService);
            var payers = await sut.GetPayersForRange(new DateTimeOffset(), new DateTimeOffset());
            Assert.Equal(2, payers.Count());
        }

        public static class StripeServiceBuilder 
        {
            public static StripeService BuildStripeService(
                List<Invoice> testInvoices = null,
                List<User> users = null,
                List<Refund> refunds = null,
                List<Charge> charges = null,
                BalanceTransaction balanceTransaction = null,
                Mock<InvoiceService> invoiceSerivceMock = null
                )
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
                var invoices = new StripeList<Invoice>() { Data = testInvoices ?? new List<Invoice>() };
                invoiceService.SetupSequence(x => x.ListAsync(It.IsAny<InvoiceListOptions>(), null, new CancellationToken()))
                    .ReturnsAsync(invoices)
                    .ReturnsAsync(new StripeList<Invoice>() { Data = new List<Invoice>() }); // Need two returns in sequence to account for paging
                stripeWrapper.Setup(x => x.InvoiceService).Returns(invoiceSerivceMock?.Object ?? invoiceService.Object);
                var userService = new Mock<IUserService>();
                userService.Setup(x => x.GetUsersFromStripeIds(It.IsAny<IEnumerable<string>>())).Returns(users ?? new List<User>());
                var refundService = new Mock<RefundService>();
                refundService.Setup(x => x.ListAsync(It.IsAny<RefundListOptions>(), null, new CancellationToken()))
                    .ReturnsAsync(new StripeList<Refund> { Data = refunds ?? new List<Refund>() });
                stripeWrapper.Setup(x => x.RefundService).Returns(refundService.Object);
                var chargeService = new Mock<ChargeService>();
                chargeService.Setup(x => x.ListAsync(It.IsAny<ChargeListOptions>(), null, new CancellationToken()))
                    .ReturnsAsync(new StripeList<Charge> { Data = charges ?? new List<Charge>() });
                chargeService.Setup(x => x.GetAsync(It.IsAny<string>(), null, null, new CancellationToken()))
                .ReturnsAsync(charges?.First());
                stripeWrapper.Setup(x => x.ChargeService).Returns(chargeService.Object);
                var balanceTransactionService = new Mock<BalanceTransactionService>();
                balanceTransactionService.Setup(x => x.GetAsync(It.IsAny<string>(), null, null, new CancellationToken()))
                    .ReturnsAsync(balanceTransaction);
                stripeWrapper.Setup(x => x.BalanceTransactionService).Returns(balanceTransactionService.Object);
                var sut = new StripeService(Options.Create(new Models.StripeConfig()), userService.Object, stripeWrapper.Object, factory);
                return sut;
            }
        }
    }
}
