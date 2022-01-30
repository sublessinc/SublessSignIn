using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Subless.Models;
using Subless.Services;
using SublessSignIn.Models;

namespace SublessSignIn.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CreatorController : ControllerBase
    {
        private readonly ICreatorService _creatorService;
        private readonly IUserService userService;
        private readonly IHitService hitService;
        private readonly IPaymentLogsService paymentLogsService;
        private readonly ILogger _logger;
        public CreatorController(ICreatorService creatorService, ILoggerFactory loggerFactory, IUserService userService, IHitService hitService, IPaymentLogsService paymentLogsService)
        {
            _creatorService = creatorService ?? throw new ArgumentNullException(nameof(creatorService));
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
            this.hitService = hitService ?? throw new ArgumentNullException(nameof(hitService));
            this.paymentLogsService = paymentLogsService ?? throw new ArgumentNullException(nameof(paymentLogsService));
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
                _logger.LogWarning(e, "Unauthorized user attempted to get creator data");
                return Unauthorized("Not a creator account");
            }
        }

        [HttpPut()]
        public async Task<ActionResult<Creator>> UpdateCreator(Creator creator)
        {
            var cognitoId = userService.GetUserClaim(HttpContext.User);
            if (cognitoId == null)
            {
                return Unauthorized();
            }
            try
            {
                return Ok(await _creatorService.UpdateCreator(cognitoId, creator));
            }
            catch (UnauthorizedAccessException e)
            {
                _logger.LogWarning(e, "Unauthorized user attempted to update creator data");
                return Unauthorized("Not a creator account");
            }
        }

        [HttpGet("stats")]
        public ActionResult<IEnumerable<MontlyPaymentStats>> GetStats()
        {
            var cognitoId = userService.GetUserClaim(HttpContext.User);
            if (cognitoId == null)
            {
                return Unauthorized();
            }
            try
            {
                var creator = _creatorService.GetCreatorByCognitoid(cognitoId);
                return Ok(_creatorService.GetStatsForCreator(creator));

            }
            catch (UnauthorizedAccessException e)
            {
                _logger.LogWarning(e, "Unauthorized user attempted to get creator stats");
                return Unauthorized();
            }
        }
        
        [HttpDelete("{id}/Unlink")]
        public ActionResult Unlink(Guid id)
        {
            var cognitoId = userService.GetUserClaim(HttpContext.User);
            if (cognitoId == null)
            {
                return Unauthorized();
            }
            try
            {
                var creator = _creatorService.UnlinkCreator(cognitoId, id);
                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                _logger.LogWarning(e, "Unauthorized user attempted to unlink creator account");
                return Unauthorized();
            }
        }

        [HttpGet("Analytics")]
        public ActionResult<HistoricalStats<UserStats>> GetUserAnalytics()
        {
            var cognitoId = userService.GetUserClaim(HttpContext.User);
            if (cognitoId == null)
            {
                return Unauthorized();
            }
            try
            {
                var creator = _creatorService.GetCreatorByCognitoid(cognitoId);
                var paymentDate = paymentLogsService.GetLastPaymentDate();
                if (paymentDate == DateTime.MinValue)
                {
                    paymentDate = DateTime.UtcNow.AddMonths(-1);
                }
                var hitsThisMonth = hitService.GetCreatorHitsByDate(paymentDate, DateTime.UtcNow, creator.Id);
                var hitsLastMonth = hitService.GetCreatorHitsByDate(paymentDate.AddMonths(-1), paymentDate, creator.Id);
                return Ok(CreatorStatsExtensions.GetHistoricalCreatorStats(hitsThisMonth, hitsLastMonth));
            }
            catch (UnauthorizedAccessException e)
            {
                _logger.LogWarning(e, "Unauthorized user attempted to get creator stats");
                return Unauthorized();
            }
        }
        [HttpGet("statscsv")]
        public ActionResult<string> GetStatsCsv()
        {
            var cognitoId = userService.GetUserClaim(HttpContext.User);
            if (cognitoId == null)
            {
                return Unauthorized();
            }
            try
            {
                var creator = _creatorService.GetCreatorByCognitoid(cognitoId);
                var stats = _creatorService.GetStatsForCreator(creator);

                MemoryStream ms = new MemoryStream();
                StreamWriter sw = new StreamWriter(ms);
                using (var csv = new CsvWriter(sw, CultureInfo.InvariantCulture))
                {
                    csv.WriteHeader<MontlyPaymentStats>();
                    csv.NextRecord();
                    csv.WriteRecords(stats);
                    csv.Flush();
                    ms.Seek(0, SeekOrigin.Begin);
                    StreamReader reader = new StreamReader(ms);
                    return reader.ReadToEnd();
                }



            }
            catch (UnauthorizedAccessException e)
            {
                _logger.LogWarning(e, "Unauthorized user attempted to get creator stats");

                return Unauthorized();
            }
        }
    }
}
