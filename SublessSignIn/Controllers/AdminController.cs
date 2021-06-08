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

        public AdminController(IAdministrationService administrationService)
        {
            this.administrationService = administrationService ?? throw new ArgumentNullException(nameof(administrationService));
        }
        
        [HttpPost("setadminwithkey")]
        public ActionResult SetAdminFromCode(Guid key) 
        {
            var cognitoId = User.FindFirst("cognito:username")?.Value;
            administrationService.ActivateAdminWithKey(key, cognitoId);
            return Ok();
        }

        [TypeFilter(typeof(AdminAuthorizationFilter))]
        [HttpPost("setadmin")]
        public ActionResult SetAdmin(Guid userId)
        {
            administrationService.ActivateAdmin(userId);
            return Ok();
        }
    }
}
