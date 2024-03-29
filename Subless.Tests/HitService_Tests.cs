using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Subless.Data;
using Subless.Models;
using Subless.Services.Services;
using Xunit;

namespace Subless.Tests
{
    public class HitService_Tests
    {

        [Fact]
        public void GetCreatorFromPartnerAndUri_WithNoMatchingPattern_ReturnsNull()
        {
            //arrange
            var sut = HitServiceBuilder();
            var uri = new Uri("http://www.partner.com/foobar/testuser");

            //act
            var creator = sut.GetCreatorFromPartnerAndUri(uri, ValidPartner());

            //assert
            Assert.Null(creator);
        }

        [Fact]
        public void GetCreatorFromPartnerAndUri_WithMatchingPattern_ReturnsCreator()
        {
            //arrange            
            var uri = new Uri("http://www.partner.com/profile/testuser");
            var creatorId = new Guid();
            var sut = HitServiceBuilder(creatorId);

            //act
            var creator = sut.GetCreatorFromPartnerAndUri(uri, ValidPartner());

            //assert
            Assert.Equal(creatorId, creator.Value);
        }

        [Fact]
        public void GetCreatorFromPartnerAndUri_WithMatchingPattern2_ReturnsCreator()
        {
            //arrange            
            var uri = new Uri("http://www.partner.com/pictures/testuser");
            var creatorId = new Guid();
            var sut = HitServiceBuilder(creatorId);

            //act
            var creator = sut.GetCreatorFromPartnerAndUri(uri, ValidPartner());

            //assert
            Assert.Equal(creatorId, creator.Value);
        }

        [Fact]
        public void GetCreatorFromPartnerAndUri_WithMatchingPattern3_ReturnsCreator()
        {
            //arrange            
            var uri = new Uri("http://www.partner.com/testuser/stories");
            var creatorId = new Guid();
            var sut = HitServiceBuilder(creatorId);

            //act
            var creator = sut.GetCreatorFromPartnerAndUri(uri, ValidPartner());

            //assert
            Assert.Equal(creatorId, creator.Value);
        }

        [Fact]
        public void GetCreatorFromPartnerAndUri_WithNestedPattern_ReturnsNull()
        {
            //arrange            
            var uri = new Uri("http://www.partner.com/fakepath/morefakepath/testuser/stories");
            var creatorId = new Guid();
            var sut = HitServiceBuilder(creatorId);

            //act
            var creator = sut.GetCreatorFromPartnerAndUri(uri, ValidPartner());

            //assert
            Assert.Null(creator);
        }

        [Fact]
        public void GetCreatorFromPartnerAndUri_WithDoubledPattern_ReturnsFirstCreator()
        {
            //arrange            
            var uri = new Uri("http://www.partner.com/pictures/testuser/pictures/testuser/");
            var creatorId = new Guid();
            var sut = HitServiceBuilder(creatorId);

            //act
            var creator = sut.GetCreatorFromPartnerAndUri(uri, ValidPartner());

            //assert
            Assert.Equal(creatorId, creator.Value);
        }

        [Fact]
        public void GetCreatorFromPartnerAndUri_WithDoubledNestedPattern_ReturnsFirstCreator()
        {
            //arrange            
            var uri = new Uri("http://www.partner.com/testuser/stories/testuser/stories");
            var creatorId = new Guid();
            var sut = HitServiceBuilder(creatorId);

            //act
            var creator = sut.GetCreatorFromPartnerAndUri(uri, ValidPartner());

            //assert
            Assert.Equal(creatorId, creator.Value);
        }

        public HitService HitServiceBuilder(Guid guid = new Guid())
        {
            var creatorService = new Mock<ICreatorService>();
            creatorService
                .Setup(i => i.GetCachedCreatorFromPartnerAndUsername(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns<string, Guid>((x, y) =>
                    new Creator()
                    {
                        PartnerId = y,
                        Username = x,
                        UserId = guid
                    });

            var serviceProvider = new ServiceCollection()
                .AddLogging(x =>
                {
                    x.AddSimpleConsole();
                })
                .BuildServiceProvider();
            var factory = serviceProvider.GetService<ILoggerFactory>();
            var logger = factory.CreateLogger<HitService>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(() => logger);

            return new HitService(
                new Mock<IUserService>().Object,
                new Mock<IUserRepository>().Object,
                new Mock<IHitRepository>().Object,
                creatorService.Object,
                new Mock<IPartnerService>().Object,
                Options.Create(new FeatureConfig()),
                mockLoggerFactory.Object);
        }

        public Partner ValidPartner()
        {
            return new Partner
            {
                CognitoAppClientId = "CognitoClient",
                PayPalId = "PayPalId",
                Sites = new List<Uri> { new Uri("https://partner.com") }.ToArray(),
                UserPattern = "http://www.partner.com/profile/{creator};http://www.partner.com/pictures/{creator};http://www.partner.com/{creator}/stories"
            };
        }
    }
}
