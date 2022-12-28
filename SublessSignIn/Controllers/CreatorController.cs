using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Subless.Models;
using Subless.Services.Services;
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
        private readonly IUsageService _usageService;
        private readonly ILogger _logger;
        public CreatorController(
            ICreatorService creatorService,
            ILoggerFactory loggerFactory,
            IUserService userService,
            IHitService hitService,
            IPaymentLogsService paymentLogsService,
            IUsageService usageService)
        {
            _creatorService = creatorService ?? throw new ArgumentNullException(nameof(creatorService));
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
            this.hitService = hitService ?? throw new ArgumentNullException(nameof(hitService));
            this.paymentLogsService = paymentLogsService ?? throw new ArgumentNullException(nameof(paymentLogsService));
            _usageService = usageService ?? throw new ArgumentNullException(nameof(usageService));
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
                return Ok(_creatorService.GetCreatorsByCognitoid(cognitoId));
            }
            catch (UnauthorizedAccessException e)
            {
                _logger.LogWarning(e, "Unauthorized user attempted to get creator data");
                return Unauthorized("Not a creator account");
            }
        }

        [HttpPut()]
        public async Task<ActionResult<IEnumerable<Creator>>> UpdateCreator(Creator creator)
        {
            var cognitoId = userService.GetUserClaim(HttpContext.User);
            if (cognitoId == null)
            {
                return Unauthorized();
            }
            try
            {
                await _creatorService.UpdateCreator(cognitoId, creator);
                return Ok(_creatorService.GetCreatorsByCognitoid(cognitoId));
            }
            catch (UnauthorizedAccessException e)
            {
                _logger.LogWarning(e, "Unauthorized user attempted to update creator data");
                return Unauthorized("Not a creator account");
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
        public ActionResult<HistoricalStats<CreatorStats>> GetUserAnalytics()
        {
            var cognitoId = userService.GetUserClaim(HttpContext.User);
            if (cognitoId == null)
            {
                return Unauthorized();
            }
            try
            {
                var creators = _creatorService.GetCreatorsByCognitoid(cognitoId);
                var creatorIds = creators.Select(x => x.Id);

                var payments = creatorIds.Select(paymentLogsService.GetLastPayment).Where(x=>x != null);
                    var lastPayment = payments?.OrderByDescending(x=>x.PaymentPeriodEnd)?.FirstOrDefault();
                    CreatorStats hitsThisMonth;
                    CreatorStats hitsLastMonth;
                    if (lastPayment == null)
                    {

                        // DEPRECATED
                        var paymentDate = paymentLogsService.GetLastPaymentDate();
                        if (paymentDate == DateTimeOffset.MinValue)
                        {
                            paymentDate = DateTimeOffset.UtcNow.AddMonths(-1);
                        }
                        hitsThisMonth = hitService.GetCreatorStats(paymentDate, DateTimeOffset.UtcNow, creatorIds, cognitoId);
                        hitsLastMonth = hitService.GetCreatorStats(paymentDate.AddMonths(-1), paymentDate, creatorIds, cognitoId);
                        // END DEPRECATED
                    }
                    else
                    {
                        hitsThisMonth = hitService.GetCreatorStats(lastPayment.PaymentPeriodEnd, DateTimeOffset.UtcNow, creatorIds, cognitoId);
                        hitsLastMonth = hitService.GetCreatorStats(lastPayment.PaymentPeriodStart, lastPayment.PaymentPeriodEnd, creatorIds, cognitoId);
                    }
                    if (creators.First().UserId != null)
                    {
                        _usageService.SaveUsage(UsageType.CreatorStats, (Guid)creators.First().UserId);
                    }
                    return Ok(new HistoricalStats<CreatorStats>
                    {
                        LastMonth = hitsLastMonth,
                        thisMonth = hitsThisMonth
                    });
            }
            catch (UnauthorizedAccessException e)
            {
                _logger.LogWarning(e, "Unauthorized user attempted to get creator stats");
                return Unauthorized();
            }
            return NotFound();

        }

        [HttpGet("statscsv")]
        public ActionResult<Dictionary<string,string>> GetStatsCsv()
        {
            var results = new Dictionary<string, string>();
            var cognitoId = userService.GetUserClaim(HttpContext.User);
            if (cognitoId == null)
            {
                return Unauthorized();
            }
            try
            {
                var creators = _creatorService.GetCreatorsByCognitoid(cognitoId);
                foreach (var creator in creators)
                {
                    var stats = _creatorService.GetStatsForCreator(creator);

                    var ms = new MemoryStream();
                    var sw = new StreamWriter(ms);
                    using (var csv = new CsvWriter(sw, CultureInfo.InvariantCulture))
                    {
                        csv.WriteHeader<MonthlyPaymentStats>();
                        csv.NextRecord();
                        csv.WriteRecords(stats);
                        csv.Flush();
                        ms.Seek(0, SeekOrigin.Begin);
                        var reader = new StreamReader(ms);
                        results.Add(creator.Username, reader.ReadToEnd());
                    }
                }
                return results;
            }
            catch (UnauthorizedAccessException e)
            {
                _logger.LogWarning(e, "Unauthorized user attempted to get creator stats");

                return Unauthorized();
            }
        }

        [HttpPut("terms")]
        public ActionResult AcceptTerms()
        {
            var cognitoId = userService.GetUserClaim(HttpContext.User);
            _creatorService.AcceptTerms(cognitoId);
            return Ok();
        }

        [HttpGet("RecentFeed")]
        public ActionResult<IEnumerable<HitView>> RecentFeed()
        {
            var cognitoId = userService.GetUserClaim(HttpContext.User);
            if (cognitoId == null)
            {
                return Unauthorized();
            }
            var views = new List<HitView>();
            try
            {
                var creators = _creatorService.GetCreatorsByCognitoid(cognitoId);
                foreach (var creator in creators)
                {
                    views.AddRange(hitService.GetRecentCreatorContent(creator.Id, cognitoId));
                }
            }
            catch (UnauthorizedAccessException e)
            {
                _logger.LogWarning(e, "Unauthorized user attempted to get creator stats");

                return Unauthorized();
            }
            return Ok(views.OrderByDescending(x=>x.Timestamp));
        }

        [HttpGet("TopFeed")]
        public ActionResult<IEnumerable<ContentHitCount>> TopFeed()
        {
            var cognitoId = userService.GetUserClaim(HttpContext.User);
            var views = new List<ContentHitCount>();
            if (cognitoId == null)
            {
                return Unauthorized();
            }
            try
            {
                var creators = _creatorService.GetCreatorsByCognitoid(cognitoId);
                foreach (var creator in creators)
                {
                    views.AddRange(hitService.GetTopCreatorContent(creator.Id, cognitoId));
                }
            }
            catch (UnauthorizedAccessException e)
            {
                _logger.LogWarning(e, "Unauthorized user attempted to get creator stats");

                return Unauthorized();
            }
            return Ok(views.OrderByDescending(x=>x.Hits));
        }

        [HttpGet("message")]
        public ActionResult<CreatorMessage> CreatorMessage()
        {
            var cognitoId = userService.GetUserClaim(HttpContext.User);
            if (cognitoId == null)
            {
                return Unauthorized();
            }
            try
            {
                var creators = _creatorService.GetCreatorsByCognitoid(cognitoId);
                foreach (var creator in creators)
                {
                    var message = _creatorService.GetCreatorMessage(creator.Id);
                    return Ok(message);
                }
                return NotFound();
            }
            catch (UnauthorizedAccessException e)
            {
                _logger.LogWarning(e, "Unauthorized user attempted to get message");

                return Unauthorized();
            }
        }

        [HttpGet("message/whitelist")]
        public ActionResult<List<string>> MessageUriWhitelist()
        {
            var cognitoId = userService.GetUserClaim(HttpContext.User);
            if (cognitoId == null)
            {
                return Unauthorized();
            }
            try
            {
                var creator = _creatorService.GetCreatorsByCognitoid(cognitoId);
                return Ok(_creatorService.GetUriWhitelist());
            }
            catch (UnauthorizedAccessException e)
            {
                _logger.LogWarning(e, "Unauthorized user attempted to get message");

                return Unauthorized();
            }
        }

        [HttpPost("message")]
        public ActionResult<CreatorMessage> SetCreatorMessage([FromBody] MessageViewModel message)
        {
            var cognitoId = userService.GetUserClaim(HttpContext.User);
            if (cognitoId == null)
            {
                return Unauthorized();
            }
            try
            {
                var creators = _creatorService.GetCreatorsByCognitoid(cognitoId);
                foreach (var creator in creators)
                {
                    return Ok(_creatorService.SetCreatorMessage(creator.Id, message.Message));
                }
                return NotFound();
            }
            catch (InputInvalidException e)
            {
                _logger.LogError(e, "Invalid creator message input");
                return new StatusCodeResult(406);
            }
            catch (NotSupportedException e)
            {
                _logger.LogError(e, "Invalid creator message input");
                return new StatusCodeResult(406);
            }
            catch (UnauthorizedAccessException e)
            {
                _logger.LogWarning(e, "Unauthorized user attempted to set message");

                return Unauthorized();
            }
        }
    }
}
