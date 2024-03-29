﻿using System;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;

namespace Subless.Services.ErrorHandling
{
    public static class LoggerSinkConfigurationExtensions
    {
        public static LoggerConfiguration SuppressionFilter(
            this LoggerSinkConfiguration lsc,
            Action<LoggerSinkConfiguration> writeTo)
        {

            return LoggerSinkConfiguration.Wrap(lsc, sink =>
                new LogSuppressionWrapper(sink), writeTo, LevelAlias.Minimum, null);
        }
    }
}
