using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Subless.Services;
using Subless.Services.Services;
using System;

namespace SublessSignIn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class AdminController : ControllerBase
    {
        private readonly IAdministrationService administrationService;
        private readonly IUserService userService;

        public AdminController(IAdministrationService administrationService, IUserService userService)
        {
            this.administrationService = administrationService ?? throw new ArgumentNullException(nameof(administrationService));
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        [HttpPost("setadminwithkey")]
        public ActionResult SetAdminFromCode([FromQuery] Guid key)
        {
            var cognitoId = userService.GetUserClaim(HttpContext.User);
            administrationService.ActivateAdminWithKey(key, cognitoId);
            return Ok();
        }



        [TypeFilter(typeof(AdminAuthorizationFilter))]
        [HttpPost("setadmin")]
        public ActionResult SetAdmin([FromQuery] Guid userId)
        {
            administrationService.ActivateAdmin(userId);
            return Ok();
        }


        [TypeFilter(typeof(AdminAuthorizationFilter))]
        [HttpDelete("demoteuser")]
        public IActionResult DemoteUser([FromQuery] Guid userId)
        {
            userService.DemoteUser(userId);
            return Ok();
        }

        [TypeFilter(typeof(AdminAuthorizationFilter))]
        [HttpGet("userCapabilties")]
        public IActionResult GetUserCapabilities([FromQuery] Guid? userId = null, string? cognitoId = null)
        {
            if (userId != null)
            {
                cognitoId = userService.GetUser(userId.Value)?.CognitoId;
            }
           if (cognitoId == null)
            {
                return NotFound();
            }
            var user = userService.GetUserByCognitoId(cognitoId);
            return Ok(user);
        }
    }
}
