using System;
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

        public CalculatorController(ICalculatorService calculatorService)
        {
            _calculatorService = calculatorService ?? throw new ArgumentNullException(nameof(calculatorService));
        }

        [HttpGet()]
        public CalculatorResult CalculateOverRange(DateTime start, DateTime end)
        {
            return _calculatorService.CaculatePayoutsOverRange(start, end);

        }
    }
}
