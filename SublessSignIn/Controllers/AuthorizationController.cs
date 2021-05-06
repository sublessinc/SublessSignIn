
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
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
        public ActionResult<Redirection> GetPath()
        {
            var cognitoId = User.FindFirst("cognito:username")?.Value;
            if (cognitoId == null)
            {
                return Unauthorized();
            }
            return _userService.LoginWorkflow(cognitoId);

        }


    }
}
