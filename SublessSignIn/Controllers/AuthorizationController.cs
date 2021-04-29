
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SublessSignIn.Models;

namespace SublessSignIn.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorizationController : ControllerBase
    {
        private readonly ILogger<AuthorizationController> _logger;
        private readonly AuthSettings _authSettings;

        public AuthorizationController(ILogger<AuthorizationController> logger, IOptions<AuthSettings> authSettings)
        {
            _logger = logger;
            _authSettings = authSettings.Value ?? throw new ArgumentNullException(nameof(authSettings));
        }

        [HttpGet("settings")]
        public AuthSettings Get()
        {
            return _authSettings;
        }
    }
}
