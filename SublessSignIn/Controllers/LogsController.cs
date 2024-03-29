﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement.Mvc;
using Subless.Models;

namespace SublessSignIn.Controllers
{

    [FeatureGate(FeatureFlags.OpenLogger)]
    [ApiController]
    [Route("api/[controller]")]
    public class LogsController : ControllerBase
    {
        ILogger<LogsController> logger;
        public LogsController(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<LogsController>();
        }
        [HttpPost]
        public void Log(FrontEndLog log)
        {
            LogLevel level;
            var downgrade = "";
            // Downgrade frontend errors to warnings
            if (log.Level > 4)
            {
                downgrade = $"Downgraded from {log.Level} - ";
                log.Level = 4;
            }
            switch (log.Level)
            {
                case 0:
                    level = LogLevel.Trace;
                    break;
                case 1:
                    level = LogLevel.Debug;
                    break;
                case 2:
                case 3:
                    level = LogLevel.Information;
                    break;
                case 4:
                    level = LogLevel.Warning;
                    break;
                case 5:
                    level = LogLevel.Error;
                    break;
                case 6:
                case 7:
                    level = LogLevel.Critical;
                    break;
                default:
                    level = LogLevel.Information;
                    break;
            }
            logger.Log(level, $"FrontEnd - {downgrade}{log.Message}");

        }
    }
}
