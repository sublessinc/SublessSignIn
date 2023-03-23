using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Subless.Models;
using Subless.PayoutCalculator;
using Subless.Services.Services;
using Xunit;
namespace Subless.Tests {
    public class TemplatedEmailService_Tests {
        private readonly Mock<IOptions<CalculatorConfiguration>> calculatorConfigurationOptionsMock = new Mock<IOptions<CalculatorConfiguration>>();
        private readonly Mock<IOptions<AuthSettings>> authOptionsMock = new Mock<IOptions<AuthSettings>>();
        private readonly Mock<IEmailService> emailServiceMock = new Mock<IEmailService>();
        private readonly Mock<ICognitoService> cognitoServiceMock = new Mock<ICognitoService>();
        private readonly Mock<ICreatorService> creatorServiceMock = new Mock<ICreatorService>();
        private readonly Mock<IPartnerService> partnerServiceMock = new Mock<IPartnerService>();
        private readonly Mock<IUserService> userServiceMock = new Mock<IUserService>();
        private readonly Mock<ILoggerFactory> loggerFactoryMock = new Mock<ILoggerFactory>();

        private string _body = null;
        private string _to = null;
        private string _subject = null;
        private string _from = null;
        
        public TemplatedEmailService_Tests() {
            calculatorConfigurationOptionsMock
                .Setup(o => o.Value)
                .Returns(new CalculatorConfiguration { Domain = "https://CalculatorConfigurationTestDomain.com" });
            
            authOptionsMock
                .Setup(o => o.Value)
                .Returns(new AuthSettings() { Domain = "https://authsettingstestdomain.com" });

            cognitoServiceMock
                .Setup(o => o.GetCognitoUserEmail(It.IsAny<string>()))
                .Returns(Task.FromResult("anything"));

            emailServiceMock
                .Setup(o => o.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback((string body, string to, string subject, string from) => {
                    _body = body;
                    _to = to;
                    _subject = subject;
                    _from = from;
                });
        }

        [Fact]
        public void GenerateEmailBodyForIdleWithHistoryEmail_WithHits_BuildsCreatorLinkListWithSubstituions() {
            var hits = new List<Hit> {
                new Hit {
                    Id = Guid.NewGuid(),
                    CognitoId = "hitCognitoId1",
                    CreatorId = Guid.NewGuid(),
                    PartnerId = Guid.NewGuid(),
                },
                new Hit {
                    Id = Guid.NewGuid(),
                    CognitoId = "hitCognitoId2",
                    CreatorId = Guid.NewGuid(),
                    PartnerId = Guid.NewGuid(),
                }
            };

            partnerServiceMock
                .Setup(o => o.GetPartner(hits[0].PartnerId))
                .Returns(new Partner {
                    UserPattern = "http://localhost:5000/uriContent/{creator}",
                    Favicon = new Uri("http://favicon1.com")
                });
            
            partnerServiceMock
                .Setup(o => o.GetPartner(hits[1].PartnerId))
                .Returns(new Partner {
                    UserPattern = "http://localhost:5000/uriContent/{creator}",
                    Favicon = new Uri("http://favicon2.com")
                });

            creatorServiceMock
                .Setup(o => o.GetCreator(hits[0].CreatorId))
                .Returns(new Creator {
                    Username = "creator1"
                });
            
            creatorServiceMock
                .Setup(o => o.GetCreator(hits[1].CreatorId))
                .Returns(new Creator {
                    Username = "creator2"
                });

            var service = new TemplatedEmailService(
                calculatorConfigurationOptionsMock.Object,
                authOptionsMock.Object,
                emailServiceMock.Object,
                cognitoServiceMock.Object,
                creatorServiceMock.Object,
                partnerServiceMock.Object,
                userServiceMock.Object,
                loggerFactoryMock.Object);

            service.SendIdleWithHistoryEmail("cognitoId", hits);

            Assert.Contains("http://localhost:5000/uriContent/creator1", _body);
            Assert.Contains("http://localhost:5000/uriContent/creator2", _body);
            Assert.Contains("http://favicon1.com", _body);
            Assert.Contains("http://favicon2.com", _body);
            Assert.Contains("https://authsettingstestdomain.com", _body);
        }
    }
}
