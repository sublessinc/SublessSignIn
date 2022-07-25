using System;
using Serilog;
using Serilog.Configuration;

namespace SublessSignIn.ErrorHandling
{
    public static class LoggerSinkConfigurationExtensions
    {
        public static LoggerConfiguration SuppressionFilter(
            this LoggerSinkConfiguration lsc,
            Action<LoggerSinkConfiguration> writeTo)
        {
            return LoggerSinkConfiguration.Wrap(
                lsc,
                wrapped => new LogSuppressionWrapper(wrapped),
                writeTo);
        }
    }
}
