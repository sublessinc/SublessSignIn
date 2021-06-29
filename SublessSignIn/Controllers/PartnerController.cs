using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Subless.Models;
using Subless.Services;

namespace SublessSignIn.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PartnerController : ControllerBase
    {
        private readonly IPartnerService _partnerService;
        private readonly ILogger _logger;
        //this is a weird place to get this from, but it'll work. Probs split it out later
        private readonly StripeConfig _settings;
        public PartnerController(IPartnerService partnerService, IOptions<StripeConfig> authSettings, ILoggerFactory loggerFactory)
        {
            _partnerService = partnerService ?? throw new ArgumentNullException(nameof(partnerService));
            _logger = loggerFactory?.CreateLogger<PartnerController>() ?? throw new ArgumentNullException(nameof(loggerFactory));
            _settings = authSettings.Value ?? throw new ArgumentNullException(nameof(authSettings));


        }

        [HttpPost("CreatorRegister")]
        public ActionResult<string> GetCreatorActivationLink([FromQuery] string username)
        {
            var scope = User.Claims.FirstOrDefault(x => x.Type == "scope")?.Value;
            var cognitoClientId = User.Claims.FirstOrDefault(x => x.Type == "client_id")?.Value;
            _logger.LogInformation($"Partner {cognitoClientId} registering creator {username}");
            if (scope == null || !scope.Contains("creator.register") || !scope.Contains(_settings.Domain) || cognitoClientId == null)
            {
                _logger.LogError($"Unauthorized user registration Scope{scope}, username:{username}, clientId: {cognitoClientId}");
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

    }
}
