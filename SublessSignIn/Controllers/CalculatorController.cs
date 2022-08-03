using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Profiling;
using Subless.Models;
using Subless.Services;
using Subless.Services.Services;

namespace SublessSignIn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [TypeFilter(typeof(AdminAuthorizationFilter))]
    public class CalculatorController : ControllerBase
    {
        private readonly ICalculatorService _calculatorService;
        private readonly IPaymentService _paymentService;

        public CalculatorController(ICalculatorService calculatorService, IPaymentService paymentService)
        {
            _calculatorService = calculatorService ?? throw new ArgumentNullException(nameof(calculatorService));
            _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
        }

        [HttpGet()]
        public ActionResult<CalculatorResult> CalculateOverRange(
            [FromQuery] DateTimeOffset start,
            [FromQuery] DateTimeOffset end
            )
        {
            using (MiniProfiler.Current.Step("CalculatorExecute"))
            {

                start = start.ToUniversalTime();
                end = end.ToUniversalTime();
                return Ok(_calculatorService.CaculatePayoutsOverRange(start, end));
            }
        }

        [HttpGet("forusers")]
        public ActionResult<CalculatorResult> CalculateOverRangeForUsers(
            [FromQuery] DateTimeOffset start,
            [FromQuery] DateTimeOffset end,
            [FromBody] List<Guid> selectedUserIds
            )
        {
            start = start.ToUniversalTime();
            end = end.ToUniversalTime();
            return Ok(_calculatorService.CaculatePayoutsOverRange(start, end, selectedUserIds));
        }


        [HttpPost()]
        public ActionResult ExecutePayoutOverRange([FromQuery] DateTimeOffset start,
            [FromQuery] DateTimeOffset end)
        {
            start = start.ToUniversalTime();
            end = end.ToUniversalTime();
            _paymentService.ExecutePayments(start, end, null);
            return Ok();

        }

        [HttpPost("forusers")]
        public ActionResult ExecutePayoutOverRangeForUsers([FromQuery] DateTimeOffset start,
            [FromQuery] DateTimeOffset end,
            [FromBody] List<Guid> selectedUserIds)
        {
            start = start.ToUniversalTime();
            end = end.ToUniversalTime();
            _paymentService.ExecutePayments(start, end, selectedUserIds);
            return Ok();
        }

        [HttpPost("QueueCalculator")]
        public ActionResult QueueCalculation(
            [FromQuery] DateTimeOffset start,
            [FromQuery] DateTimeOffset end)
        {
            start = start.ToUniversalTime();
            end = end.ToUniversalTime();
            var id = _calculatorService.QueueCalculation(start, end);
            return Ok(id);
        }

        [HttpPost("QueuePayment")]
        public ActionResult QueuePayment(
            [FromQuery] DateTimeOffset start,
            [FromQuery] DateTimeOffset end)
        {
            start = start.ToUniversalTime();
            end = end.ToUniversalTime();
            var id = _paymentService.QueuePayment(start, end);
            return Ok(id);
        }

        [HttpGet("GetQueuedCalculation")]
        public ActionResult GetQueuedResult(
            [FromQuery] Guid id)
        {
            var result = _calculatorService.GetQueuedResult(id);
            return Ok(result);
        }

        [HttpPost("EmailIdleCreators")]
        public ActionResult EmailIdleCreators(DateTimeOffset start, DateTimeOffset end)
        {
            start = start.ToUniversalTime();
            end = end.ToUniversalTime();
            _paymentService.QueueIdleEmail(start, end);
            return Ok();
        }
    }
}
