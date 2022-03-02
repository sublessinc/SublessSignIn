using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;

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
            .WriteTo.Console();
            return log;
        }

        private static LoggingLevelSwitch GetLogLevelSwitch()
        {
            var level = Environment.GetEnvironmentVariable("Logging__LogLevel__Default");
            Enum.TryParse(level, out LogEventLevel parsedlevel);
            return new LoggingLevelSwitch(parsedlevel);
        }
    }
}
