using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subless.Services
{
    public class HealthCheckFailureException : Exception
    {
        public HealthCheckFailureException(string message) : base(message)
        {
            
        }
    }
}
