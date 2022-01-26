using SublessSignIn.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SublessSignIn.Models
{
    public class HistoricalStats<T>
    {
        public T thisMonth { get; set; }
        public T LastMonth { get; set; }
    }
}
