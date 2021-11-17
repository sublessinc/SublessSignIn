using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Subless.Models;
using Subless.Services;
using Subless.Services.Extensions;
using SublessSignIn.Models;

namespace SublessSignIn.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PartnerController : ControllerBase
    {
        private readonly IPartnerService _partnerService;
        private readonly IUserService _userService;
        private readonly ICreatorService creatorService;
        private readonly ILogger _logger;
        //this is a weird place to get this from, but it'll work. Probs split it out later
        private readonly StripeConfig _settings;
        public PartnerController(IPartnerService partnerService, IUserService userService, ICreatorService creatorService, IOptions<StripeConfig> authSettings, ILoggerFactory loggerFactory)
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
            _logger = loggerFactory?.CreateLogger<PartnerController>() ?? throw new ArgumentNullException(nameof(loggerFactory));
            _settings = authSettings.Value ?? throw new ArgumentNullException(nameof(authSettings));


        }

        [HttpPost("CreatorRegister")]
        public ActionResult<string> GetCreatorActivationLink([FromQuery] string username)
        {
            if (PartnerService.InvalidUsernameCharacters.Any(chr => username.Contains(chr)))
            {
                return BadRequest("Username contains one of the following invalid characters" + PartnerService.InvalidUsernameCharacters.Concat(" "));
            }
            var scope = User.Claims.FirstOrDefault(x => x.Type == "scope")?.Value;
            var cognitoClientId = User.Claims.FirstOrDefault(x => x.Type == "client_id")?.Value;
            _logger.LogInformation($"Partner {cognitoClientId} registering creator {HttpUtility.UrlEncode(username)}");
            if (scope == null || !scope.Contains("creator.register") || !scope.Contains(_settings.Domain) || cognitoClientId == null)
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
        public ActionResult<IEnumerable<PartnerViewCreator>> GetCreatorsForPartner()
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
            var creators = creatorService.GetCreatorsByPartnerId(partner.Id);
            return Ok(creators.Select(x => x.ToPartnerView()));
        }

        [HttpGet("Creators/{id}")]
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
        public ActionResult<Guid> NewPartner([FromBody] Partner partner)
        {
            _partnerService.CreatePartner(partner);
            return Ok(partner.Id);
        }

        [TypeFilter(typeof(AdminAuthorizationFilter))]
        [HttpGet()]
        public ActionResult<IEnumerable<Partner>> GetPartners()
        {
            return Ok(_partnerService.GetPartners());
        }

        [TypeFilter(typeof(AdminAuthorizationFilter))]
        [HttpPut()]
        public ActionResult UpdatePartner([FromBody] Partner partner)
        {
            _partnerService.UpdatePartner(partner);
            return Ok();
        }

        [HttpGet("config")]
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

        [HttpPut("payPalId")]
        public ActionResult<PartnerResponse> UpdatePayPalId([FromQuery] string payPalId)
        {
            var userClaim = _userService.GetUserClaim(this.User);
            var user = _userService.GetUserByCognitoId(userClaim);
            if (user == null || !user.Partners.Any())
            {
                return Unauthorized("Attemped to access forbidden zone");
            }
            var partner = _partnerService.UpdatePartnerPayPalId(user.Partners.First().Id, payPalId);

            return Ok(partner.GetViewModel());
        }


    }
}
