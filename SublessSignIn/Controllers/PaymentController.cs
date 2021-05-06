using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace SublessSignIn.Controllers
{
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        public PaymentController()
        {
            Console.WriteLine("I'm trying");
        }

        [HttpPost("pay")]
        [EnableCors("Unrestricted")]
        [Authorize]
        public void Setup(Hit hit)
        {

            Console.WriteLine($"Hit from : {User.FindFirst("cognito:username")?.Value} on {hit.currentUrl}");
        }

        public class Hit
        {
            public string currentUrl { get; set; }
        }

    }
}
