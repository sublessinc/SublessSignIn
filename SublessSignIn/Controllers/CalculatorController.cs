using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            start = start.ToUniversalTime();
            end = end.ToUniversalTime();
            return Ok(_calculatorService.CaculatePayoutsOverRange(start, end));
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
    }
}
