using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private IPartnerService _partnerService;
        private ILogger _logger;
        //this is a weird place to get this from, but it'll work. Probs split it out later
        private StripeConfig _settings;
        public PartnerController(IPartnerService partnerService, IOptions<StripeConfig> authSettings, ILoggerFactory loggerFactory)
        {
            _partnerService = partnerService ?? throw new ArgumentNullException(nameof(partnerService));
            _logger = loggerFactory?.CreateLogger<PartnerController>() ?? throw new ArgumentNullException(nameof(loggerFactory));
            _settings = authSettings.Value ?? throw new ArgumentNullException(nameof(authSettings));


        }

        [HttpGet]
        public ActionResult<string> GetCreatorActivationLink([FromQuery] string username)
        {
            var scope = User.Claims.FirstOrDefault(x=> x.Type == "scope")?.Value;
            var cognitoClientId = User.Claims.FirstOrDefault(x=> x.Type == "client_id")?.Value;            
            if (scope == null || !scope.Contains("creator.register") || !scope.Contains(_settings.Domain) || cognitoClientId == null)
            {
                _logger.LogError($"Unauthorized user registration Scope{scope}, username:{username}, clientId: {cognitoClientId}");
                return Unauthorized();
            }          

            return _partnerService.GenerateCreatorActivationLink(cognitoClientId, username).ToString();
        }
    }
}
