using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Subless.Models;
using Subless.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var sut = PaymentServiceBuilder(s3Service: s3Service);


            //Act
            sut.ExecutePayments(DateTimeOffset.UtcNow.AddMonths(-1), DateTimeOffset.UtcNow);

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
                    Payment = 100,
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
            var sut = PaymentServiceBuilder(
                stripe: stripeService,
                s3Service: s3Service,
                hitService: hitService,
                creatorService: creatorService,
                partnerService: partnerService
                );
            //Act
            sut.ExecutePayments(DateTimeOffset.UtcNow.AddMonths(-1), DateTimeOffset.UtcNow);

            //Assert
            Assert.NotEmpty(allPayments); // We should have a payment directed at subless, even if it's zero
            Assert.Equal(3, allPayments.Count); //Subless, Partner, Creator
        }

        [Fact]
        public void CalculatorService_WithInActiveCreator_SkipsInactive()
        {
            //Arrange
            var allPayments = new Dictionary<string, double>();
            var stripeService = StripeServiceBuilder(new List<Payer>
            {
                new Payer()
                {
                    Payment = 100,
                    UserId = Guid.NewGuid()
                }
            });
            var inActiveCreatorId = Guid.NewGuid();
            var activeCreatorId = Guid.NewGuid();
            var hit = new List<Hit> {
                new Hit { CognitoId = "test",  CreatorId = activeCreatorId },
                new Hit { CognitoId = "inactive", CreatorId = inActiveCreatorId },
            };
            var hitService = HitServiceBuilder(hit);
            var s3Service = new Mock<IFileStorageService>();
            s3Service.Setup(x => x.WritePaymentsToCloudFileStore(It.IsAny<Dictionary<string, double>>()))
                .Callback<Dictionary<string, double>>(y =>
                {
                    allPayments = y;
                });
            var creatorService = new Mock<ICreatorService>();
            creatorService.Setup(x => x.GetCreator(It.Is<Guid>(x => x == activeCreatorId))).Returns(new Creator() { Id = activeCreatorId, PayPalId = "test" });
            creatorService.Setup(x => x.GetCreator(It.Is<Guid>(x => x == inActiveCreatorId))).Returns(new Creator() { Id = inActiveCreatorId, PayPalId = null });

            var partnerService = PartnerServiceBuilder();
            var sut = PaymentServiceBuilder(
                stripe: stripeService,
                s3Service: s3Service,
                hitService: hitService,
                creatorService: creatorService,
                partnerService: partnerService
                );
            //Act
            sut.ExecutePayments(DateTimeOffset.UtcNow.AddMonths(-1), DateTimeOffset.UtcNow);

            //Assert
            Assert.NotEmpty(allPayments); // We should have a payment directed at subless, even if it's zero
            Assert.Equal(3, allPayments.Count); //Subless, Partner, Active creator, not inactive creator
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
                    Payment = 100,
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
            var sut = PaymentServiceBuilder(
                stripe: stripeService,
                s3Service: s3Service,
                hitService: hitService,
                creatorService: creatorService,
                partnerService: partnerService
                );
            //Act
            sut.ExecutePayments(DateTimeOffset.UtcNow.AddMonths(-1), DateTimeOffset.UtcNow);

            //Assert
            Assert.NotEmpty(allPayments); // We should have a payment directed at subless
            Assert.Equal(CalculatorService.SublessFraction, allPayments[sut.SublessPayPalId]);
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
                    Payment = 67,
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
            var sut = PaymentServiceBuilder(
                stripe: stripeService,
                s3Service: s3Service,
                hitService: hitService,
                creatorService: creatorService,
                partnerService: partnerService
                );
            //Act
            sut.ExecutePayments(DateTimeOffset.UtcNow.AddMonths(-1), DateTimeOffset.UtcNow);

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
                    Payment = 67,
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
            var sut = PaymentServiceBuilder(
                stripe: stripeService,
                s3Service: s3Service,
                hitService: hitService,
                creatorService: creatorService,
                partnerService: partnerService
                );
            //Act
            sut.ExecutePayments(DateTimeOffset.UtcNow.AddMonths(-1), DateTimeOffset.UtcNow);

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
                    Payment = 940,
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
            creatorService.Setup(x => x.GetCreator(It.Is<Guid>(x => x == creator1))).Returns(new Creator() { PayPalId = "Creator1", Id = creator1 });
            creatorService.Setup(x => x.GetCreator(It.Is<Guid>(x => x == creator2))).Returns(new Creator() { PayPalId = "Creator2", Id = creator2 });
            var partnerService = PartnerServiceBuilder("Partner");
            var sut = PaymentServiceBuilder(
                stripe: stripeService,
                s3Service: s3Service,
                hitService: hitService,
                creatorService: creatorService,
                partnerService: partnerService
                );
            //Act
            sut.ExecutePayments(DateTimeOffset.UtcNow.AddMonths(-1), DateTimeOffset.UtcNow);

            //Assert
            Assert.NotEmpty(allPayments); // We should have a payment directed at subless
            Assert.Equal(3.68, allPayments["Creator1"]);
            Assert.Equal(3.68, allPayments["Creator2"]);
            Assert.Equal(1.84, allPayments["Partner"]);
        }

        [Fact]
        public void GetCreatorPayees_WithSevenCreators_CalculatesCreatorCut()
        {
            //arrange
            var sut = CalculatorServiceBuilder(creatorService: CreatorServicePayPalMatcherBuilder());
            var creator1 = new KeyValuePair<Guid, int>(Guid.NewGuid(), 33);
            var creator2 = new KeyValuePair<Guid, int>(Guid.NewGuid(), 22);
            var creator3 = new KeyValuePair<Guid, int>(Guid.NewGuid(), 11);
            var creator4 = new KeyValuePair<Guid, int>(Guid.NewGuid(), 44);
            var creator5 = new KeyValuePair<Guid, int>(Guid.NewGuid(), 55);
            var creator6 = new KeyValuePair<Guid, int>(Guid.NewGuid(), 11);
            var creator7 = new KeyValuePair<Guid, int>(Guid.NewGuid(), 22);

            var hits = new Dictionary<Guid, int>();
            hits.Add(creator1.Key, creator1.Value);
            hits.Add(creator2.Key, creator2.Value);
            hits.Add(creator3.Key, creator3.Value);
            hits.Add(creator4.Key, creator4.Value);
            hits.Add(creator5.Key, creator5.Value);
            hits.Add(creator6.Key, creator6.Value);
            hits.Add(creator7.Key, creator7.Value);

            //act
            var result = sut.GetCreatorPayees(4.55, hits, 198, CalculatorService.PartnerFraction, CalculatorService.SublessFraction);

            //assert
            Assert.Equal(7, result.Count());

            //these are .01 lower than the sheet in some cases due to rounding down
            Assert.Equal(.5945, result.Single(x => x.PayPalId == creator1.Key.ToString()).Payment);
            Assert.Equal(.3963, result.Single(x => x.PayPalId == creator2.Key.ToString()).Payment);
            Assert.Equal(.1981, result.Single(x => x.PayPalId == creator3.Key.ToString()).Payment);
            Assert.Equal(.7927, result.Single(x => x.PayPalId == creator4.Key.ToString()).Payment);
            Assert.Equal(.9908, result.Single(x => x.PayPalId == creator5.Key.ToString()).Payment);
            Assert.Equal(.1981, result.Single(x => x.PayPalId == creator6.Key.ToString()).Payment);
            Assert.Equal(.3963, result.Single(x => x.PayPalId == creator7.Key.ToString()).Payment);
        }

        [Fact]
        public void GetCreatorPayees_WithOneCreators_CalculatesCreatorCut()
        {
            //arrange
            var sut = CalculatorServiceBuilder(creatorService: CreatorServicePayPalMatcherBuilder());
            var creator1 = new KeyValuePair<Guid, int>(Guid.NewGuid(), 1);

            var hits = new Dictionary<Guid, int>();
            hits.Add(creator1.Key, creator1.Value);


            //act
            var result = sut.GetCreatorPayees(4.55, hits, 1, CalculatorService.PartnerFraction, CalculatorService.SublessFraction);

            //assert
            Assert.Single(result);

            //these are .01 lower than the sheet in some cases due to rounding down
            Assert.Equal(3.5672, result.Single(x => x.PayPalId == creator1.Key.ToString()).Payment);

        }

        [Fact]
        public void GetCreatorPayees_WithTwoCreators_CalculatesCreatorCut()
        {
            //arrange
            var sut = CalculatorServiceBuilder(creatorService: CreatorServicePayPalMatcherBuilder());
            var creator1 = new KeyValuePair<Guid, int>(Guid.NewGuid(), 1);
            var creator2 = new KeyValuePair<Guid, int>(Guid.NewGuid(), 1);


            var hits = new Dictionary<Guid, int>();
            hits.Add(creator1.Key, creator1.Value);
            hits.Add(creator2.Key, creator2.Value);


            //act
            var result = sut.GetCreatorPayees(9.40, hits, 2, CalculatorService.PartnerFraction, CalculatorService.SublessFraction);

            //assert
            Assert.Equal(2, result.Count());

            //these are .01 lower than the sheet in some cases due to rounding down
            Assert.Equal(3.6848, result.Single(x => x.PayPalId == creator1.Key.ToString()).Payment);
            Assert.Equal(3.6848, result.Single(x => x.PayPalId == creator2.Key.ToString()).Payment);
        }

        [Fact]
        public void GetCreatorPayees_WithTwoUnequalCreators_CalculatesCreatorCut()
        {
            //arrange
            var sut = CalculatorServiceBuilder(creatorService: CreatorServicePayPalMatcherBuilder());
            var creator1 = new KeyValuePair<Guid, int>(Guid.NewGuid(), 10);
            var creator2 = new KeyValuePair<Guid, int>(Guid.NewGuid(), 5);


            var hits = new Dictionary<Guid, int>();
            hits.Add(creator1.Key, creator1.Value);
            hits.Add(creator2.Key, creator2.Value);


            //act
            var result = sut.GetCreatorPayees(4.55, hits, 15, CalculatorService.PartnerFraction, CalculatorService.SublessFraction);

            //assert
            Assert.Equal(2, result.Count());

            //these are .01 lower than the sheet in some cases due to rounding down
            Assert.Equal(2.3781, result.Single(x => x.PayPalId == creator1.Key.ToString()).Payment);
            Assert.Equal(1.189, result.Single(x => x.PayPalId == creator2.Key.ToString()).Payment);
        }

        [Fact]
        public void GetCreatorPayees_WithTwoCrossPartnerCreators_CalculatesCreatorCut()
        {
            //arrange
            var sut = CalculatorServiceBuilder(creatorService: CreatorServicePayPalMatcherBuilder());
            var creator1 = new KeyValuePair<Guid, int>(Guid.NewGuid(), 10);
            var creator2 = new KeyValuePair<Guid, int>(Guid.NewGuid(), 5);


            var hits = new Dictionary<Guid, int>();
            hits.Add(creator1.Key, creator1.Value);
            hits.Add(creator2.Key, creator2.Value);


            //act
            var result = sut.GetCreatorPayees(4.55, hits, 15, CalculatorService.PartnerFraction, CalculatorService.SublessFraction);

            //assert
            Assert.Equal(2, result.Count());

            //these are .01 lower than the sheet in some cases due to rounding down
            Assert.Equal(2.3781, result.Single(x => x.PayPalId == creator1.Key.ToString()).Payment);
            Assert.Equal(1.189, result.Single(x => x.PayPalId == creator2.Key.ToString()).Payment);
        }

        [Fact]
        public void GetPartnerPayees_WithTwoCrossPartnerCreators_CalculatesPartnerCut()
        {
            //arrange

            var creator1 = new KeyValuePair<Guid, int>(Guid.NewGuid(), 10);
            var creator2 = new KeyValuePair<Guid, int>(Guid.NewGuid(), 5);
            var partner1 = Guid.NewGuid();
            var partner2 = Guid.NewGuid();

            var hits = new Dictionary<Guid, int>();
            hits.Add(creator1.Key, creator1.Value);
            hits.Add(creator2.Key, creator2.Value);
            var partners = new Dictionary<Guid, List<Guid>>();
            partners.Add(partner1, new List<Guid> { creator1.Key });
            partners.Add(partner2, new List<Guid> { creator2.Key });
            var sut = CalculatorServiceBuilder(creatorService: CreatorServicePayPalMatcherBuilder(partners), partnerService: PartnerServiceCreatorMatcherBuilder(partners));

            //act
            var result = sut.GetPartnerPayees(4.55, hits, 15, CalculatorService.PartnerFraction, CalculatorService.SublessFraction);

            //assert
            Assert.Equal(2, result.Count());

            //these are .01 lower than the sheet in some cases due to rounding down
            Assert.Equal(.5945, result.Single(x => x.PayPalId == partner1.ToString()).Payment);
            Assert.Equal(.2972, result.Single(x => x.PayPalId == partner2.ToString()).Payment);
        }

        [Fact]
        public void GetPartnerPayees_WithTenCrossPartnerCreators_CalculatesPartnerCut()
        {
            //arrange

            var creator1 = new KeyValuePair<Guid, int>(Guid.NewGuid(), 33);
            var creator2 = new KeyValuePair<Guid, int>(Guid.NewGuid(), 22);
            var creator3 = new KeyValuePair<Guid, int>(Guid.NewGuid(), 11);
            var creator4 = new KeyValuePair<Guid, int>(Guid.NewGuid(), 44);
            var creator5 = new KeyValuePair<Guid, int>(Guid.NewGuid(), 55);
            var creator6 = new KeyValuePair<Guid, int>(Guid.NewGuid(), 11);
            var creator7 = new KeyValuePair<Guid, int>(Guid.NewGuid(), 22);

            var hits = new Dictionary<Guid, int>();
            hits.Add(creator1.Key, creator1.Value);
            hits.Add(creator2.Key, creator2.Value);
            hits.Add(creator3.Key, creator3.Value);
            hits.Add(creator4.Key, creator4.Value);
            hits.Add(creator5.Key, creator5.Value);
            hits.Add(creator6.Key, creator6.Value);
            hits.Add(creator7.Key, creator7.Value);

            var partner1 = Guid.NewGuid();
            var partner2 = Guid.NewGuid();

            var partners = new Dictionary<Guid, List<Guid>>();
            partners.Add(partner1, new List<Guid> { creator1.Key, creator2.Key, creator3.Key, creator4.Key, creator5.Key });
            partners.Add(partner2, new List<Guid> { creator6.Key, creator7.Key });
            var sut = CalculatorServiceBuilder(creatorService: CreatorServicePayPalMatcherBuilder(partners), partnerService: PartnerServiceCreatorMatcherBuilder(partners));

            //act
            var result = sut.GetPartnerPayees(4.55, hits, 198, CalculatorService.PartnerFraction, CalculatorService.SublessFraction);

            //assert
            Assert.Equal(2, result.Count());

            //these are .01 lower than the sheet in some cases due to rounding down
            Assert.Equal(.7428, result.Single(x => x.PayPalId == partner1.ToString()).Payment);
            Assert.Equal(.1485, result.Single(x => x.PayPalId == partner2.ToString()).Payment);
        }

        [Fact]
        public void GetPartnerPayees_WithTwoCreatorsOnePartner_CalculatesPartnerCut()
        {
            //arrange

            var creator1 = new KeyValuePair<Guid, int>(Guid.NewGuid(), 10);
            var creator2 = new KeyValuePair<Guid, int>(Guid.NewGuid(), 5);
            var partner1 = Guid.NewGuid();

            var hits = new Dictionary<Guid, int>();
            hits.Add(creator1.Key, creator1.Value);
            hits.Add(creator2.Key, creator2.Value);
            var partners = new Dictionary<Guid, List<Guid>>();
            partners.Add(partner1, new List<Guid> { creator1.Key, creator2.Key });
            var sut = CalculatorServiceBuilder(creatorService: CreatorServicePayPalMatcherBuilder(partners), partnerService: PartnerServiceCreatorMatcherBuilder(partners));

            //act
            var result = sut.GetPartnerPayees(4.55, hits, 15, CalculatorService.PartnerFraction, CalculatorService.SublessFraction);

            //assert
            Assert.Single(result);

            //these are .01 lower than the sheet in some cases due to rounding down
            Assert.Equal(.8917, result.Single(x => x.PayPalId == partner1.ToString()).Payment);
        }

        [Fact]
        public void Payer_WithNoHits_RollsOverPayment()
        {
            //Arrange
            var allPayments = new Dictionary<string, double>();
            var mockStripe = new Mock<IStripeService>();
            mockStripe.Setup(x => x.RolloverPaymentForIdleCustomer(It.IsAny<string>()));

            var stripeService = StripeServiceBuilder(new List<Payer>
            {
                new Payer()
                {
                    Payment = 940,
                    UserId = Guid.NewGuid()
                }
            }, mockStripe);

            var hit = new List<Hit>
            {

            };
            var hitService = HitServiceBuilder(hit);
            var s3Service = new Mock<IFileStorageService>();
            s3Service.Setup(x => x.WritePaymentsToCloudFileStore(It.IsAny<Dictionary<string, double>>()))
                .Callback<Dictionary<string, double>>(y =>
                {
                    allPayments = y;
                });
            var creatorService = new Mock<ICreatorService>();
            var partnerService = PartnerServiceBuilder("Partner");
            var sut = PaymentServiceBuilder(
                stripe: stripeService,
                s3Service: s3Service,
                hitService: hitService,
                creatorService: creatorService,
                partnerService: partnerService
                );
            //Act
            sut.ExecutePayments(DateTimeOffset.UtcNow.AddMonths(-1), DateTimeOffset.UtcNow);

            //Assert
            Assert.Empty(allPayments);
            mockStripe.Verify(mock => mock.RolloverPaymentForIdleCustomer(It.IsAny<string>()), Times.Once());
        }


        [Fact]
        public void Rollover_WithOtherPayers_ExecutesOtherPayments()
        {

            //Arrange

            var creator = new Creator()
            {
                Id = Guid.NewGuid(),
                PayPalId = "paypal"
            };
            var partners = new Dictionary<Guid, List<Guid>>();
            var partner = Guid.NewGuid();
            partners.Add(partner, new List<Guid> { creator.Id });

            var payer = new User()
            {
                CognitoId = "cognito",
                Id = Guid.NewGuid()
            };
            var allPayments = new Dictionary<string, double>();
            var mockStripe = new Mock<IStripeService>();
            mockStripe.Setup(x => x.RolloverPaymentForIdleCustomer(It.IsAny<string>()));

            var stripeService = StripeServiceBuilder(new List<Payer>
            {
                new Payer()
                {
                    Payment = 940,
                    UserId = payer.Id
                },
                new Payer()
                {
                    Payment = 940,
                    UserId = Guid.NewGuid()
                }
            }, mockStripe);

            var hit = new List<Hit>
            {
                new Hit()
                {
                    CreatorId = creator.Id,
                    CognitoId = payer.CognitoId
                }
            };
            var hitService = new Mock<IHitService>();
            hitService.Setup(x => x.GetHitsByDate(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), payer.Id)).Returns(hit); 
            var creatorService = new Mock<ICreatorService>();
            creatorService.Setup(x => x.GetCreator(creator.Id)).Returns(creator);
            var partnerService = PartnerServiceBuilder("Partner");
            var sut = CalculatorServiceBuilder(
                stripe: stripeService,
                hitService: hitService,
                creatorService: creatorService,
                partnerService: partnerService
                );
            //Act
            var result = sut.CaculatePayoutsOverRange(DateTimeOffset.UtcNow.AddMonths(-1), DateTimeOffset.UtcNow);

            //Assert
            Assert.Equal(3, result.AllPayouts.Count);
            Assert.Single(result.IdleCustomerStripeIds);
        }


        [Fact]
        public void Execution_WithUserFilter_IgnoresOthers()
        {

            //Arrange

            var creator = new Creator()
            {
                Id = Guid.NewGuid(),
                PayPalId = "paypal"
            };
            var partners = new Dictionary<Guid, List<Guid>>();
            var partner = Guid.NewGuid();
            partners.Add(partner, new List<Guid> { creator.Id });

            var payer = new User()
            {
                CognitoId = "cognito",
                Id = Guid.NewGuid()
            };
            var payer2 = new User()
            {
                CognitoId = "cognito2",
                Id = Guid.NewGuid()
            };
            var allPayments = new Dictionary<string, double>();
            var mockStripe = new Mock<IStripeService>();
            mockStripe.Setup(x => x.RolloverPaymentForIdleCustomer(It.IsAny<string>()));

            var stripeService = StripeServiceBuilder(new List<Payer>
            {
                new Payer()
                {
                    Payment = 940,
                    UserId = payer.Id
                },
                new Payer()
                {
                    Payment = 940,
                    UserId = payer2.Id
                }
            }, mockStripe);

            var hit = new List<Hit>
            {
                new Hit()
                {
                    CreatorId = creator.Id,
                    CognitoId = payer.CognitoId
                }
            };
            var hit2 = new List<Hit>
            {
                new Hit()
                {
                    CreatorId = creator.Id,
                    CognitoId = payer2.CognitoId
                }
            };
            var hitService = new Mock<IHitService>();
            hitService.Setup(x => x.GetHitsByDate(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), payer.Id)).Returns(hit);
            hitService.Setup(x => x.GetHitsByDate(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), payer2.Id)).Returns(hit2);
            var creatorService = new Mock<ICreatorService>();
            creatorService.Setup(x => x.GetCreator(creator.Id)).Returns(creator);
            var userService = new Mock<IUserService>();
            userService.Setup(x => x.GetUser(payer.Id)).Returns(payer);
            userService.Setup(x => x.GetUser(payer2.Id)).Returns(payer2);
            var partnerService = PartnerServiceBuilder("Partner");
            var sut = CalculatorServiceBuilder(
                stripe: stripeService,
                hitService: hitService,
                creatorService: creatorService,
                partnerService: partnerService,
                userService: userService
                );
            //Act
            var result = sut.CaculatePayoutsOverRange(DateTimeOffset.UtcNow.AddMonths(-1), DateTimeOffset.UtcNow, new List<Guid> {  payer2.Id });

            //Assert
            Assert.Equal(3, result.AllPayouts.Count);
            Assert.DoesNotContain(result.PaymentsPerPayer, x=>x.Key==payer.CognitoId);
        }


        private PaymentService PaymentServiceBuilder(
            Mock<IStripeService> stripe = null,
            Mock<IHitService> hitService = null,
            Mock<IFileStorageService> s3Service = null,
            Mock<ICreatorService> creatorService = null,
            Mock<IPartnerService> partnerService = null
            )
        {

            var serviceProvider = new ServiceCollection()
                .AddLogging(x =>
                {
                    x.AddSimpleConsole();
                })
                .BuildServiceProvider();

            var factory = serviceProvider.GetService<ILoggerFactory>();

            var logger = factory.CreateLogger<PaymentService>();

            var mockLoggerFactory = new Mock<ILoggerFactory>();
            mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(() => logger);


            //TODO split these tests, till then, both of these are the SUT
            var calculatorService = CalculatorServiceBuilder(stripe, hitService, creatorService, partnerService);

            return new PaymentService(
                stripe?.Object ?? StripeServiceBuilder().Object,
                new Mock<IPaymentLogsService>().Object,
                s3Service?.Object ?? new Mock<IFileStorageService>().Object,
                CreateOptions(),
                new Mock<IPaymentEmailService>().Object,
                calculatorService,
                mockLoggerFactory.Object
                );
        }

        private CalculatorService CalculatorServiceBuilder(
            Mock<IStripeService> stripe = null,
            Mock<IHitService> hitService = null,
            Mock<ICreatorService> creatorService = null,
            Mock<IPartnerService> partnerService = null,
            Mock<IUserService> userService = null
            )
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging(x =>
                {
                    x.AddSimpleConsole();
                })
                .BuildServiceProvider();
            var factory = serviceProvider.GetService<ILoggerFactory>();

            var logger = factory.CreateLogger<CalculatorService>();

            var mockLoggerFactory = new Mock<ILoggerFactory>();
            mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(() => logger);
            var baseUserSerivce = new Mock<IUserService>();
            baseUserSerivce.Setup(x => x.GetUser(It.IsAny<Guid>())).Returns(new User() { CognitoId = "cognito" });

            //TODO split these tests, till then, both of these are the SUT
            var calculatorService = new CalculatorService(
                stripe?.Object ?? StripeServiceBuilder().Object,
                hitService?.Object ?? new Mock<IHitService>().Object,
                creatorService?.Object ?? new Mock<ICreatorService>().Object,
                partnerService?.Object ?? new Mock<IPartnerService>().Object,
                new Mock<IPaymentLogsService>().Object,
                userService?.Object ?? baseUserSerivce.Object,
                CreateOptions(),
                mockLoggerFactory.Object
                );
            return calculatorService;
        }


        private IOptions<StripeConfig> CreateOptions()
        {
            return Options.Create<StripeConfig>(new StripeConfig()
            {
                SublessPayPalId = "sublesspayPal"
            });
        }

        private Mock<IStripeService> StripeServiceBuilder(List<Payer> payers = null, Mock<IStripeService> stripeService = null)
        {
            var service = stripeService ?? new Mock<IStripeService>();
            service.Setup(x => x.GetInvoicesForRange(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>())).Returns(payers ?? new List<Payer>());
            return service;
        }

        private Mock<IHitService> HitServiceBuilder(List<Hit> hits)
        {
            var service = new Mock<IHitService>();
            service.Setup(x => x.GetHitsByDate(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<Guid>())).Returns(hits);
            return service;
        }

        private Mock<ICreatorService> CreatorServiceBuilder(string payPalId = "TestCreator")
        {
            var service = new Mock<ICreatorService>();
            service.Setup(x => x.GetCreator(It.IsAny<Guid>())).Returns(new Creator() { PayPalId = payPalId });
            return service;
        }

        private Mock<ICreatorService> CreatorServicePayPalMatcherBuilder(Dictionary<Guid, List<Guid>> partners = null)
        {
            var service = new Mock<ICreatorService>();
            service.Setup(x => x.GetCreator(It.IsAny<Guid>())).Returns<Guid>(x =>
            {
                var creator = new Creator();
                creator.PayPalId = x.ToString();
                if (partners != null)
                {
                    creator.PartnerId = partners.Single(y => y.Value.Contains(x)).Key;
                }
                return creator;
            });
            return service;
        }

        private Mock<IPartnerService> PartnerServiceBuilder(string PayPalId = "TestPartner")
        {
            var service = new Mock<IPartnerService>();
            service.Setup(x => x.GetPartner(It.IsAny<Guid>())).Returns(new Partner() { PayPalId = PayPalId, Sites = new List<Uri> { new Uri("https://google.com") }.ToArray() });
            return service;
        }

        private Mock<IPartnerService> PartnerServiceCreatorMatcherBuilder(Dictionary<Guid, List<Guid>> partners)
        {
            var service = new Mock<IPartnerService>();
            service.Setup(x => x.GetPartner(It.IsAny<Guid>())).Returns<Guid>(x => new Partner() { PayPalId = x.ToString(), Sites = new List<Uri> { new Uri("https://google.com") }.ToArray() });
            return service;
        }
    }
}


