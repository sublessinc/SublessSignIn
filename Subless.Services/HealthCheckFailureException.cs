using System;

namespace Subless.Services
{
    public class HealthCheckFailureException : Exception
    {
        public HealthCheckFailureException(string message) : base(message)
        {

        }
    }
}
