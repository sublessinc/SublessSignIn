using System;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Subless.Services.ErrorHandling;
namespace Subless.Services.Services
{
    public static class LoggerConfig
    {
        public static Logger GetLogger()
        {
            return ConfigureLogger().CreateLogger();
        }

        public static LoggerConfiguration ConfigureLogger()
        {
            var log = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .MinimumLevel.ControlledBy(GetLogLevelSwitch())
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .WriteTo.SuppressionFilter(x => x.Console());
            return log;
        }

        private static LoggingLevelSwitch GetLogLevelSwitch()
        {
            var level = Environment.GetEnvironmentVariable("Logging__LogLevel__Default");
            if (!Enum.TryParse(level, out LogEventLevel parsedlevel))
            {
                parsedlevel = LogEventLevel.Warning;
                Console.WriteLine("Warning: Were not able to get the Logging__LogLevel__Default env var to set our LogEventLevel.");
            }

            return new LoggingLevelSwitch(parsedlevel);
        }
    }
}
