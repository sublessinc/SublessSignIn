using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            var cognitoId = User.FindFirst("cognito:username")?.Value;
            administrationService.ActivateAdminWithKey(key, cognitoId);
            return Ok();
        }

        [HttpGet("user")]
        public ActionResult<Guid> GetUserId()
        {
            var cognitoId = User.FindFirst("cognito:username")?.Value;
            return Ok(userService.GetUserByCognitoId(cognitoId).Id);
        }

        [TypeFilter(typeof(AdminAuthorizationFilter))]
        [HttpPost("setadmin")]
        public ActionResult SetAdmin([FromQuery] Guid userId)
        {
            administrationService.ActivateAdmin(userId);
            return Ok();
        }
    }
}
