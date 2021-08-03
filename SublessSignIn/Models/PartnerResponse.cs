using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Subless.Models;

namespace SublessSignIn.Models
{
    public class PartnerResponse
    {
        public string PayoneerId { get; set; }
        public Uri Site { get; set; }
        public string UserPattern { get; set; }
    }


}
