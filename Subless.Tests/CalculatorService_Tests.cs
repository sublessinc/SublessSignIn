using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Subless.Models;
using Subless.PayoutCalculator;
using Subless.Services;
using Xunit;

namespace Subless.Tests
{
    public class CalculatorService_Tests
    {
        [Fact]
        public void CalculatorService_WithNoPayment_SendsNoPayment()
        {
            //Arrange
            var allPayments = new Dictionary<string, double>();
            var s3Service = new Mock<IFileStorageService>();
            s3Service.Setup(x => x.WritePaymentsToCloudFileStore(It.IsAny<Dictionary<string, double>>()))
                .Callback<Dictionary<string, double>>(y =>
                {
                    allPayments = y;
                });
            var sut = CalculatorServiceBuilder(s3Service: s3Service);


            //Act
            sut.CalculatePayments(DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow);

            //Assert
            Assert.Empty(allPayments); // We shouldn't be paying anyone
            Assert.Equal(0, allPayments.Sum(payment => payment.Value)); //We shouldn't be paying anyone

        }

        [Fact]
        public void CalculatorService_WithOneView_SendsToUsCreatorAndPartner()
        {
            //Arrange
            var allPayments = new Dictionary<string, double>();
            var stripeService = StripeServiceBuilder(new List<Payer>
            {
                new Payer()
                {
                    Payment = 1,
                    UserId = Guid.NewGuid()
                }
            });
            var hit = new List<Hit> { new Hit { CognitoId = "test" } };
            var hitService = HitServiceBuilder(hit);
            var s3Service = new Mock<IFileStorageService>();
            s3Service.Setup(x => x.WritePaymentsToCloudFileStore(It.IsAny<Dictionary<string, double>>()))
                .Callback<Dictionary<string, double>>(y =>
                {
                    allPayments = y;
                });
            var creatorService = CreatorServiceBuilder();
            var partnerService = PartnerServiceBuilder();
            var sut = CalculatorServiceBuilder(
                stripe: stripeService,
                s3Service: s3Service,
                hitService: hitService,
                creatorService: creatorService,
                partnerService: partnerService
                );
            //Act
            sut.CalculatePayments(DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow);

            //Assert
            Assert.NotEmpty(allPayments); // We should have a payment directed at subless, even if it's zero
            Assert.Equal(3, allPayments.Count); //Subless, Partner, Creator
        }

        [Fact]
        public void CalculatorService_WithOneDollar_CalculatesOurCut()
        {
            //Arrange
            var allPayments = new Dictionary<string, double>();
            var stripeService = StripeServiceBuilder(new List<Payer>
            {
                new Payer()
                {
                    Payment = 1,
                    UserId = Guid.NewGuid()
                }
            });
            var hit = new List<Hit> { new Hit { CognitoId = "test" } };
            var hitService = HitServiceBuilder(hit);
            var s3Service = new Mock<IFileStorageService>();
            s3Service.Setup(x => x.WritePaymentsToCloudFileStore(It.IsAny<Dictionary<string, double>>()))
                .Callback<Dictionary<string, double>>(y =>
                {
                    allPayments = y;
                });
            var creatorService = CreatorServiceBuilder();
            var partnerService = PartnerServiceBuilder();
            var sut = CalculatorServiceBuilder(
                stripe: stripeService,
                s3Service: s3Service,
                hitService: hitService,
                creatorService: creatorService,
                partnerService: partnerService
                );
            //Act
            sut.CalculatePayments(DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow);

            //Assert
            Assert.NotEmpty(allPayments); // We should have a payment directed at subless
            Assert.Equal(CalculatorService.SublessFraction, allPayments[sut.SublessPayoneerId]);
        }

        [Fact]
        public void CalculatorService_WithOneDollarAndOneView_CalculatesPartnerCut()
        {
            //Arrange
            var allPayments = new Dictionary<string, double>();
            var stripeService = StripeServiceBuilder(new List<Payer>
            {
                new Payer()
                {
                    Payment = .67,
                    UserId = Guid.NewGuid()
                }
            });
            var hit = new List<Hit> { new Hit { CognitoId = "test" } };
            var hitService = HitServiceBuilder(hit);
            var s3Service = new Mock<IFileStorageService>();
            s3Service.Setup(x => x.WritePaymentsToCloudFileStore(It.IsAny<Dictionary<string, double>>()))
                .Callback<Dictionary<string, double>>(y =>
                {
                    allPayments = y;
                });
            var creatorService = CreatorServiceBuilder();
            var partnerService = PartnerServiceBuilder("Partner");
            var sut = CalculatorServiceBuilder(
                stripe: stripeService,
                s3Service: s3Service,
                hitService: hitService,
                creatorService: creatorService,
                partnerService: partnerService
                );
            //Act
            sut.CalculatePayments(DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow);

            //Assert
            Assert.NotEmpty(allPayments); // We should have a payment directed at subless
            Assert.Equal(.13, allPayments["Partner"]);
        }


        [Fact]
        public void CalculatorService_WithOneDollarAndOneView_CalculatesCreatorCut()
        {
            //Arrange
            var allPayments = new Dictionary<string, double>();
            var stripeService = StripeServiceBuilder(new List<Payer>
            {
                new Payer()
                {
                    Payment = .67,
                    UserId = Guid.NewGuid()
                }
            });
            var hit = new List<Hit> { new Hit { CognitoId = "test" } };
            var hitService = HitServiceBuilder(hit);
            var s3Service = new Mock<IFileStorageService>();
            s3Service.Setup(x => x.WritePaymentsToCloudFileStore(It.IsAny<Dictionary<string, double>>()))
                .Callback<Dictionary<string, double>>(y =>
                {
                    allPayments = y;
                });
            var creatorService = CreatorServiceBuilder("Creator");
            var partnerService = PartnerServiceBuilder();
            var sut = CalculatorServiceBuilder(
                stripe: stripeService,
                s3Service: s3Service,
                hitService: hitService,
                creatorService: creatorService,
                partnerService: partnerService
                );
            //Act
            sut.CalculatePayments(DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow);

            //Assert
            Assert.NotEmpty(allPayments); // We should have a payment directed at subless
            Assert.Equal(.52, allPayments["Creator"]);
        }

        [Fact]
        public void CalculatorService_WithTwoCreators_CalculatesCreatorCut()
        {
            //Arrange
            var allPayments = new Dictionary<string, double>();
            var stripeService = StripeServiceBuilder(new List<Payer>
            {
                new Payer()
                {
                    Payment = 9.40,
                    UserId = Guid.NewGuid()
                }
            });
            var creator1 = Guid.NewGuid();
            var creator2 = Guid.NewGuid();
            var hit = new List<Hit> {
                new Hit {  CreatorId = creator1 } ,
                new Hit { CreatorId = creator2 }
            };
            var hitService = HitServiceBuilder(hit);
            var s3Service = new Mock<IFileStorageService>();
            s3Service.Setup(x => x.WritePaymentsToCloudFileStore(It.IsAny<Dictionary<string, double>>()))
                .Callback<Dictionary<string, double>>(y =>
                {
                    allPayments = y;
                });
            var creatorService = new Mock<ICreatorService>();
            creatorService.Setup(x => x.GetCreator(It.Is<Guid>(x => x == creator1))).Returns(new Creator() { PayoneerId = "Creator1" });
            creatorService.Setup(x => x.GetCreator(It.Is<Guid>(x => x == creator2))).Returns(new Creator() { PayoneerId = "Creator2" });
            var partnerService = PartnerServiceBuilder("Partner");
            var sut = CalculatorServiceBuilder(
                stripe: stripeService,
                s3Service: s3Service,
                hitService: hitService,
                creatorService: creatorService,
                partnerService: partnerService
                );
            //Act
            sut.CalculatePayments(DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow);

            //Assert
            Assert.NotEmpty(allPayments); // We should have a payment directed at subless
            Assert.Equal(3.68, allPayments["Creator1"]);
            Assert.Equal(3.68, allPayments["Creator2"]);
            Assert.Equal(1.84, allPayments["Partner"]);
        }

        private CalculatorService CalculatorServiceBuilder(
            Mock<IStripeService> stripe = null,
            Mock<IHitService> hitService = null,
            Mock<IFileStorageService> s3Service = null,
            Mock<ICreatorService> creatorService = null,
            Mock<IPartnerService> partnerService = null
            )
        {
            return new CalculatorService(
                stripe?.Object ?? StripeServiceBuilder().Object,
                hitService?.Object ?? new Mock<IHitService>().Object,
                creatorService?.Object ?? new Mock<ICreatorService>().Object,
                partnerService?.Object ?? new Mock<IPartnerService>().Object,
                new Mock<IPaymentLogsService>().Object,
                s3Service?.Object ?? new Mock<IFileStorageService>().Object,
                CreateOptions(),
                new Mock<ILoggerFactory>().Object
                );
        }

        private IOptions<StripeConfig> CreateOptions()
        {
            return Options.Create<StripeConfig>(new StripeConfig()
            {
                SublessPayoneerId = "sublesspayoneer"
            });
        }

        private Mock<IStripeService> StripeServiceBuilder(List<Payer> payers = null)
        {
            var service = new Mock<IStripeService>();
            service.Setup(x => x.GetInvoicesForRange(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(payers ?? new List<Payer>());
            return service;
        }

        private Mock<IHitService> HitServiceBuilder(List<Hit> hits)
        {
            var service = new Mock<IHitService>();
            service.Setup(x => x.GetHitsByDate(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).Returns(hits);
            return service;
        }

        private Mock<ICreatorService> CreatorServiceBuilder(string payoneerId = "TestCreator")
        {
            var service = new Mock<ICreatorService>();
            service.Setup(x => x.GetCreator(It.IsAny<Guid>())).Returns(new Creator() { PayoneerId = payoneerId });
            return service;
        }

        private Mock<IPartnerService> PartnerServiceBuilder(string PayoneerId = "TestPartner")
        {
            var service = new Mock<IPartnerService>();
            service.Setup(x => x.GetPartner(It.IsAny<Guid>())).Returns(new Partner() { PayoneerId = PayoneerId });
            return service;
        }
    }
}

