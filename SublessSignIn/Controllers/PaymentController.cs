using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SublessSignIn.Controllers
{
    [Route("api/[controller]")]
    public class HitController : ControllerBase
    {
        private ILogger _logger;
        public HitController(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory?.CreateLogger<HitController>() ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        [HttpPost]
        [EnableCors("Unrestricted")]
        [Authorize]
        public async Task<ActionResult> Hit()
        {
            using (var reader = new StreamReader(Request.Body))
            {
                var body = await reader.ReadToEndAsync();
                if (string.IsNullOrWhiteSpace(body))
                {
                    _logger.LogWarning("Invalid hit data recieved:{Environment.NewLine}" +
                       $"Http Request Information:{Environment.NewLine}" +
                       $"Schema:{Request.Scheme} " +
                       $"Host: {Request.Host} " +
                       $"Path: {Request.Path} " +
                       $"QueryString: {Request.QueryString}");
                    return BadRequest("No url included in hit");
                }
                if (!Uri.TryCreate(body, UriKind.RelativeOrAbsolute, out Uri hitSource))
                {
                    return BadRequest("Could not read source url");
                }
            }
            return Ok();
        }
    }
}
