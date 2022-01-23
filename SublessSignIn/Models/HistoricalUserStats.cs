using SublessSignIn.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SublessSignIn.Models
{
    public class HistoricalUserStats
    {
        public UserStats thisMonth { get; set; }
        public UserStats LastMonth { get; set; }
    }
}
