using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Subless.Models;

namespace SublessSignIn.Controllers
{

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
            switch (log.Level) {
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
            logger.Log(level, log.Message);

        }
    }
}
