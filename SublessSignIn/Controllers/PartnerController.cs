using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Subless.Models;
using Subless.Services;
using Subless.Services.Extensions;
using SublessSignIn.AuthServices;
using SublessSignIn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SublessSignIn.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PartnerController : ControllerBase
    {
        private readonly IPartnerService _partnerService;
        private readonly IUserService _userService;
        private readonly ICreatorService creatorService;
        private readonly IHitService hitService;
        private readonly IPaymentLogsService paymentLogsService;
        private readonly ICorsPolicyAccessor corsPolicyAccessor;
        private readonly IUsageService _usageService;
        private readonly ILogger _logger;
        //this is a weird place to get this from, but it'll work. Probs split it out later
        private readonly StripeConfig _settings;
        public PartnerController(
            IPartnerService partnerService,
            IUserService userService,
            ICreatorService creatorService,
            IHitService hitService,
            IPaymentLogsService paymentLogsService,
            IOptions<StripeConfig> authSettings,
            ICorsPolicyAccessor corsPolicyAccessor,
            IUsageService usageService,
            ILoggerFactory loggerFactory)
        {
            if (authSettings is null)
            {
                throw new ArgumentNullException(nameof(authSettings));
            }

            if (loggerFactory is null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            _partnerService = partnerService ?? throw new ArgumentNullException(nameof(partnerService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            this.creatorService = creatorService ?? throw new ArgumentNullException(nameof(creatorService));
            this.hitService = hitService ?? throw new ArgumentNullException(nameof(hitService));
            this.paymentLogsService = paymentLogsService ?? throw new ArgumentNullException(nameof(paymentLogsService));
            this.corsPolicyAccessor = corsPolicyAccessor;
            _usageService = usageService ?? throw new ArgumentNullException(nameof(usageService));
            _logger = loggerFactory?.CreateLogger<PartnerController>() ?? throw new ArgumentNullException(nameof(loggerFactory));
            _settings = authSettings.Value ?? throw new ArgumentNullException(nameof(authSettings));


        }

        [HttpPost("CreatorRegister")]
        [Authorize(BearerAuth.PartnerSchemeName)]
        public ActionResult<string> GetCreatorActivationLink([FromQuery] string username)
        {
            if (PartnerService.InvalidUsernameCharacters.Any(chr => username.Contains(chr, StringComparison.Ordinal)))
            {
                return BadRequest("Username contains one of the following invalid characters" + PartnerService.InvalidUsernameCharacters.Concat(" "));
            }
            var scope = User.Claims.FirstOrDefault(x => x.Type == "scope")?.Value;
            var cognitoClientId = User.Claims.FirstOrDefault(x => x.Type == "client_id")?.Value;
            _logger.LogInformation($"Partner {cognitoClientId} registering creator {HttpUtility.UrlEncode(username)}");
            if (scope == null || !scope.Contains("creator.register", StringComparison.Ordinal) || !scope.Contains(_settings.Domain, StringComparison.Ordinal) || cognitoClientId == null)
            {
                _logger.LogError($"Unauthorized user registration Scope{scope}, username:{HttpUtility.UrlEncode(username)}, clientId: {cognitoClientId}");
                return Unauthorized();
            }
            try
            {
                return _partnerService.GenerateCreatorActivationLink(cognitoClientId, username).ToString();
            }
            catch (UnauthorizedAccessException e)
            {
                _logger.LogError(e, $"Invalid partner credentials {cognitoClientId}");
                return Unauthorized("Your partner credentials are invalid");
            }
            catch (CreatorAlreadyActiveException e)
            {
                _logger.LogError(e, $"Creator attempted to activate twice {username}");
                return BadRequest("This creator is already activated on subless");
            }
        }


        [HttpGet("Creators")]
        [Authorize(BearerAuth.PartnerSchemeName)]
        public ActionResult<IEnumerable<PartnerViewCreator>> GetCreatorsForPartner([FromQuery] string username = null)
        {
            var cognitoClientId = User.Claims.FirstOrDefault(x => x.Type == "client_id")?.Value;
            if (string.IsNullOrWhiteSpace(cognitoClientId))
            {
                return Unauthorized("Provided token is not a client token");
            }
            var partner = _partnerService.GetPartnerByCognitoClientId(cognitoClientId);
            if (partner == null)
            {
                return NotFound("No partner registered for the given client ID");
            }
            IEnumerable<Creator> creators = null;
            if (username != null)
            {
                var creator = creatorService.GetCachedCreatorFromPartnerAndUsername(username, partner.Id);

                if (creator is null)
                {
                    return NotFound("No creators found for the given username");
                }
                creators = new List<Creator> { creator };
            }
            else
            {
                creators = creatorService.GetCreatorsByPartnerId(partner.Id);
            }
            return Ok(creators.Select(x => x.ToPartnerView()));
        }

        [HttpGet("Creators/{id}")]
        [Authorize(BearerAuth.PartnerSchemeName)]
        public ActionResult<IEnumerable<PartnerViewCreator>> GetCreatorByPartner(Guid id)
        {
            var cognitoClientId = User.Claims.FirstOrDefault(x => x.Type == "client_id")?.Value;
            if (string.IsNullOrWhiteSpace(cognitoClientId))
            {
                return Unauthorized("Provided token is not a client token");
            }
            var partner = _partnerService.GetPartnerByCognitoClientId(cognitoClientId);
            if (partner == null)
            {
                return NotFound("No partner registered for the given client ID");
            }
            var creator = creatorService.GetCreator(id);
            if (creator == null || creator.PartnerId != partner.Id)
            {
                return NotFound();
            }
            return Ok(creator.ToPartnerView());
        }

        [TypeFilter(typeof(AdminAuthorizationFilter))]
        [HttpPost()]
        [Authorize]
        public ActionResult<Guid> NewPartner([FromBody] Partner partner)
        {
            _partnerService.CreatePartner(partner);

            //Update the list of allowed origins
            corsPolicyAccessor.SetUnrestrictedOrigins();
            return Ok(partner.Id);
        }

        [TypeFilter(typeof(AdminAuthorizationFilter))]
        [HttpGet()]
        [Authorize]
        public ActionResult<IEnumerable<Partner>> GetPartners()
        {
            return Ok(_partnerService.GetPartners());
        }

        [TypeFilter(typeof(AdminAuthorizationFilter))]
        [HttpPut()]
        [Authorize]
        public ActionResult UpdatePartner([FromBody] Partner partner)
        {

            _partnerService.UpdatePartner(partner);
            //Update the list of allowed origins
            corsPolicyAccessor.SetUnrestrictedOrigins();
            return Ok();
        }

        [HttpGet("config")]
        [Authorize]
        public ActionResult<PartnerResponse> GetPartner()
        {
            var userClaim = _userService.GetUserClaim(this.User);
            var user = _userService.GetUserByCognitoId(userClaim);
            var partner = _partnerService.GetPartnerByAdminId(user.Id);
            if (partner == null)
            {
                return Unauthorized("Attemped to access forbidden zone");
            }
            return Ok(partner.GetViewModel());
        }

        [HttpPut("{id}")]
        [Authorize]
        public ActionResult<PartnerResponse> UpdateOwnPartner([FromBody] PartnerWriteModel partner)
        {
            var userClaim = _userService.GetUserClaim(this.User);
            var user = _userService.GetUserByCognitoId(userClaim);
            if (user == null || !user.Partners.Any() || !user.Partners.Any(x => partner.Id == x.Id))
            {
                return Unauthorized("Attemped to access forbidden zone");
            }
            var updatedPartner = _partnerService.UpdatePartnerWritableFields(partner);

            return Ok(updatedPartner.GetViewModel());
        }

        [HttpPost("WebhookTest")]
        [Authorize]
        public async Task<ActionResult<PartnerResponse>> WebhookTest()
        {
            var userClaim = _userService.GetUserClaim(this.User);
            var user = _userService.GetUserByCognitoId(userClaim);
            var partner = _partnerService.GetPartnerByAdminId(user.Id);
            if (partner == null)
            {
                return Unauthorized("Attemped to access forbidden zone");
            }
            var dummyCreator = new PartnerViewCreator()
            {
                Active = true,
                Email = "TestSublessWebhook@webhooktest.com",
                Id = Guid.NewGuid(),
                IsDeleted = false,
                PartnerId = partner.Id,
                Username = "TestSublessWebhookUsername"
            };
            return Ok(await _partnerService.CreatorChangeWebhook(dummyCreator));
        }

        [HttpGet("Analytics")]
        [Authorize]
        public ActionResult<HistoricalStats<UserStats>> GetPartnerAnalytics()
        {
            var cognitoId = _userService.GetUserClaim(HttpContext.User);
            var user = _userService.GetUserByCognitoId(cognitoId);
            if (cognitoId == null)
            {
                return Unauthorized();
            }
            var partner = _partnerService.GetPartnerByAdminId(user.Id);
            if (partner == null)
            {
                return Unauthorized("Attemped to access forbidden zone");
            }
            try
            {
                var paymentDate = paymentLogsService.GetLastPaymentDate();
                if (paymentDate == DateTimeOffset.MinValue)
                {
                    paymentDate = DateTimeOffset.UtcNow.AddMonths(-1);
                }
                var hitsThisMonth = hitService.GetPartnerStats(paymentDate, DateTimeOffset.UtcNow, partner.Id);
                var hitsLastMonth = hitService.GetPartnerStats(paymentDate.AddMonths(-1), paymentDate, partner.Id);
                _usageService.SaveUsage(UsageType.PartnerStats, user.Id);
                return Ok(new HistoricalStats<PartnerStats>()
                {
                    thisMonth = hitsThisMonth,
                    LastMonth = hitsLastMonth
                });
                
            }
            catch (UnauthorizedAccessException e)
            {
                _logger.LogWarning(e, "Unauthorized user attempted to get creator stats");
                return Unauthorized();
            }
        }

        [HttpPut("terms")]
        public ActionResult AcceptTerms()
        {
            var cognitoId = _userService.GetUserClaim(HttpContext.User);
            _partnerService.AcceptTerms(cognitoId);
            return Ok();
        }
    }
}
