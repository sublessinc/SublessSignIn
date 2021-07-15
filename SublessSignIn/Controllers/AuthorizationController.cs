
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Subless.Models;
using Subless.Services;
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

        public AuthorizationController(ILogger<AuthorizationController> logger, IOptions<AuthSettings> authSettings, IUserService userService)
        {
            _logger = logger;
            _authSettings = authSettings.Value ?? throw new ArgumentNullException(nameof(authSettings));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
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
            var cognitoId = User.FindFirst("username")?.Value;
            if (cognitoId == null)
            {
                return Unauthorized();
            }
            return _userService.LoginWorkflow(cognitoId, Activation);
        }
    }
}
