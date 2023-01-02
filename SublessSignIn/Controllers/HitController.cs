using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Subless.Models;
using Subless.Services.Services;
using SublessSignIn.Models;

namespace SublessSignIn.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HitController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IHitService _hitService;
        private readonly IUserService userService;

        public HitController(ILoggerFactory loggerFactory, IHitService hitService, IUserService userService)
        {
            _logger = loggerFactory?.CreateLogger<HitController>() ?? throw new ArgumentNullException(nameof(loggerFactory));
            _hitService = hitService ?? throw new ArgumentNullException(nameof(hitService));
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        [HttpPost]
        [EnableCors("Unrestricted")]
        [Authorize]
        public async Task<ActionResult<bool>> Hit()
        {
            try
            {
                var validHit = false;
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
                            $"QueryString: {HttpUtility.UrlEncode(Request.QueryString.Value)}");
                        return BadRequest("No url included in hit");
                    }
                    if (!Uri.TryCreate(body, UriKind.RelativeOrAbsolute, out var hitSource))
                    {
                        return BadRequest("Could not read source url");
                    }
                    if (userService.GetUserClaim(HttpContext.User) == null)
                    {
                        return Unauthorized("User claim could not be found");
                    }
                    validHit = _hitService.SaveHit(userService.GetUserClaim(HttpContext.User), hitSource);
                }
                return Ok(validHit);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to record hit");
            }
            return Ok(false);
        }

        [HttpPost("tag")]
        [EnableCors("Unrestricted")]
        [Authorize]
        public async Task<ActionResult<bool>> TagHit()
        {
            try
            {
                var validHit = false;
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
                            $"QueryString: {HttpUtility.UrlEncode(Request.QueryString.Value)}");
                        return BadRequest("No tag body included in hit");
                    }
                    var tagHit = JsonConvert.DeserializeObject<TagHitViewModel>(body);
                    if (!Uri.TryCreate(tagHit.Uri, UriKind.RelativeOrAbsolute, out var hitSource))
                    {
                        return BadRequest("Could not read source url");
                    }
                    if (userService.GetUserClaim(HttpContext.User) == null)
                    {
                        return Unauthorized("User claim could not be found");
                    }
                    validHit = _hitService.SaveHit(userService.GetUserClaim(HttpContext.User), hitSource);
                }
                return Ok(validHit);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to record hit");
            }
            return Ok(false);
        }

        [HttpPost("Test")]
        [EnableCors("Unrestricted")]
        [Authorize]
        public async Task<ActionResult<Hit>> TestHit()
        {
            Hit hit;
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
                       $"QueryString: {HttpUtility.UrlEncode(Request.QueryString.Value)}");
                    return BadRequest("No url included in hit");
                }
                if (!Uri.TryCreate(body, UriKind.RelativeOrAbsolute, out var hitSource))
                {
                    return BadRequest("Could not read source url");
                }
                if (userService.GetUserClaim(HttpContext.User) == null)
                {
                    return Unauthorized("User claim could not be found");
                }
                hit = _hitService.TestHit(userService.GetUserClaim(HttpContext.User), hitSource);
            }
            return Ok(hit);
        }
    }
}
