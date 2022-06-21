using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SublessSignIn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VersionController : ControllerBase
    {
        private readonly IVersion releaseVersion;

        public VersionController(IVersion releaseVersion)
        {
            this.releaseVersion = releaseVersion;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<string>> GetVersion()
        {
            var version = await releaseVersion.GetVersion();
            if (!string.IsNullOrEmpty(version))
            {
                return Ok(version);
            }

            return StatusCode(503, "Version not found.");
        }
    }
}