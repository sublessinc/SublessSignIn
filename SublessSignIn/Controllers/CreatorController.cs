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
        private readonly ILogger _logger;
        public CreatorController(ICreatorService creatorService, ILoggerFactory loggerFactory)
        {
            _creatorService = creatorService ?? throw new ArgumentNullException(nameof(creatorService));
            _logger = loggerFactory?.CreateLogger<PartnerController>() ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        [HttpGet()]
        public ActionResult<Creator> GetCreator()
        {
            var cognitoId = User.FindFirst("cognito:username")?.Value;
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
            var cognitoId = User.FindFirst("cognito:username")?.Value;
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
