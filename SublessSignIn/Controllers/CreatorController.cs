using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Subless.Models;
using Subless.Services;

namespace SublessSignIn.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CreatorController : ControllerBase
    {
        private readonly ICreatorService _creatorService;
        private readonly IUserService userService;
        private readonly ILogger _logger;
        public CreatorController(ICreatorService creatorService, ILoggerFactory loggerFactory, IUserService userService)
        {
            _creatorService = creatorService ?? throw new ArgumentNullException(nameof(creatorService));
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = loggerFactory?.CreateLogger<PartnerController>() ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        [HttpGet()]
        public ActionResult<Creator> GetCreator()
        {
            var cognitoId = userService.GetUserClaim(HttpContext.User);
            if (cognitoId == null)
            {
                return Unauthorized();
            }
            try
            {
                return Ok(_creatorService.GetCreatorByCognitoid(cognitoId));
            }
            catch (UnauthorizedAccessException e)
            {
                return Unauthorized("Not a creator account");
            }
        }

        [HttpPut()]
        public ActionResult<Creator> UpdateCreator(Creator creator)
        {
            var cognitoId = userService.GetUserClaim(HttpContext.User);
            if (cognitoId == null)
            {
                return Unauthorized();
            }
            try
            {
                return Ok(_creatorService.UpdateCreator(cognitoId, creator));
            }
            catch (UnauthorizedAccessException e)
            {
                return Unauthorized("Not a creator account");
            }
        }
    }
}
