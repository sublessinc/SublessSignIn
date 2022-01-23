using System;
using Microsoft.Extensions.Logging;
using Moq;
using Subless.Data;
using Subless.Models;
using Subless.Services;
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
            return new HitService(
                new Mock<IUserService>().Object,
                new Mock<IUserRepository>().Object,
                creatorService.Object,
                new Mock<IPartnerService>().Object,
                new Mock<ILoggerFactory>().Object);
        }

        public Partner ValidPartner()
        {
            return new Partner
            {
                CognitoAppClientId = "CognitoClient",
                PayPalId = "PayPalId",
                Site = new Uri("http://www.partner.com"),
                UserPattern = "http://www.partner.com/profile/{creator};http://www.partner.com/pictures/{creator};http://www.partner.com/{creator}/stories"
            };
        }
    }
}
