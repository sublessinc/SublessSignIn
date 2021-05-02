using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Subless.Models;

namespace SublessSignIn.Models
{
    public class Redirection
    {
        public RedirectionPath RedirectionPath { get; set; }
        public string SessionId { get; set; }
    }
}
