
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Subless.Models;
using Subless.Services;
using Subless.Services.Services;
using SublessSignIn.Models;

namespace SublessSignIn.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorizationController : ControllerBase
    {
        private readonly ILogger<AuthorizationController> _logger;
        private readonly AuthSettings _authSettings;
        private readonly IUserService _userService;
        private readonly IAuthService authorizationService;

        public AuthorizationController(
            ILogger<AuthorizationController> logger, 
            IOptions<AuthSettings> authSettings, 
            IUserService userService,
            IAuthService authorizationService
            )
        {
            if (authSettings is null)
            {
                throw new ArgumentNullException(nameof(authSettings));
            }

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _authSettings = authSettings.Value ?? throw new ArgumentNullException(nameof(authSettings));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            this.authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
        }

        [HttpGet("settings")]
        [EnableCors("Unrestricted")]
        public AuthSettings Get()
        {

            return _authSettings;
        }

        [Authorize]
        [HttpGet("redirect")]
        public ActionResult<Redirection> GetPath([FromHeader] string Activation)
        {
            var cognitoId = _userService.GetUserClaim(HttpContext.User);
            if (cognitoId == null)
            {
                return Unauthorized();
            }
            return authorizationService.LoginWorkflow(cognitoId, Activation);
        }

        [Authorize]
        [HttpGet("routes")]
        public ActionResult<IEnumerable<RedirectionPath>> Routes()
        {
            var cognitoId = _userService.GetUserClaim(HttpContext.User);
            if (cognitoId == null)
            {
                return Unauthorized();
            }
            return Ok(authorizationService.GetAllowedPaths(cognitoId));
        }
    }
}
