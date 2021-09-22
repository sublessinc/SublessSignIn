using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Subless.Services;

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
        public IActionResult GetUserCapabilities([FromQuery] Guid userId)
        {
            var user = userService.GetUser(userId);
            user = userService.GetUserByCognitoId(user.CognitoId);
            return Ok(user);
        }    
    }
}
