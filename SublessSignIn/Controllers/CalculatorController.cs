using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        public async Task<ActionResult<CalculatorResult>> CalculateOverRange(
            [FromQuery] DateTimeOffset start,
            [FromQuery] DateTimeOffset end
            )
        {
            using (MiniProfiler.Current.Step("CalculatorExecute"))
            {

                start = start.ToUniversalTime();
                end = end.ToUniversalTime();
                return Ok(await _calculatorService.CaculatePayoutsOverRange(start, end));
            }
        }

        [HttpGet("forusers")]
        public async Task<ActionResult<CalculatorResult>> CalculateOverRangeForUsers(
            [FromQuery] DateTimeOffset start,
            [FromQuery] DateTimeOffset end,
            [FromBody] List<Guid> selectedUserIds
            )
        {
            start = start.ToUniversalTime();
            end = end.ToUniversalTime();
            return Ok(await _calculatorService.CaculatePayoutsOverRange(start, end, selectedUserIds));
        }


        [HttpPost()]
        public async Task<ActionResult> ExecutePayoutOverRange([FromQuery] DateTimeOffset start,
            [FromQuery] DateTimeOffset end)
        {
            start = start.ToUniversalTime();
            end = end.ToUniversalTime();
            await _paymentService.ExecutePayments(start, end, null);
            return Ok();

        }

        [HttpPost("forusers")]
        public async Task<ActionResult> ExecutePayoutOverRangeForUsers([FromQuery] DateTimeOffset start,
            [FromQuery] DateTimeOffset end,
            [FromBody] List<Guid> selectedUserIds)
        {
            start = start.ToUniversalTime();
            end = end.ToUniversalTime();
            await _paymentService.ExecutePayments(start, end, selectedUserIds);
            return Ok();

        }
    }
}
