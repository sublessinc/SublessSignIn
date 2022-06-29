using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SublessSignIn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        private readonly IHealthCheck healthCheck;
        private readonly ILogger logger;

        public HealthCheckController(IHealthCheck healthCheck, ILoggerFactory loggerFactory)
        {
            this.healthCheck = healthCheck ?? throw new ArgumentNullException(nameof(healthCheck));
            this.logger = loggerFactory?.CreateLogger<IHealthCheck>() ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<bool>> IsHealthy()
        {
            bool isHealthy = false;
            try
            {
                isHealthy = await healthCheck.IsHealthy();
            }
            catch (Exception e)
            {
                logger.LogCritical(e, "HealthCheck failure");
            }
            if (isHealthy)
            {
                return Ok(true);
            }
            return StatusCode(503, false);
        }
    }
}
