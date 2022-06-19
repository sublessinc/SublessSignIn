using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace SublessSignIn
{
    public class ExceptionHandlingMiddleware
    {
        public RequestDelegate requestDelegate;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate requestDelegate, ILoggerFactory loggerFactory)
        {
            this.requestDelegate = requestDelegate ?? throw new ArgumentNullException(nameof(requestDelegate));
            _logger = loggerFactory?.CreateLogger<ExceptionHandlingMiddleware>() ?? throw new ArgumentNullException(nameof(loggerFactory));
        }
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await requestDelegate(context);
            }
            catch (Exception ex)
            {
                await HandleException(context, ex);
            }
        }

        private Task HandleException(HttpContext context, Exception ex)
        {
            if (ex is TaskCanceledException)
            {
                _logger.LogWarning("Task cancelled error occurred. This is occurring on logout in some cases.");
                _logger.LogWarning($"Path {context.Request.Path}");
                _logger.LogWarning($"Method {context.Request.Method}");
                _logger.LogWarning($"Query {context.Request.Query}");
                _logger.LogWarning($"Subject ID: {context.User.FindFirst("sub")}");
                _logger.LogWarning($"If the subject ID has been removed from the sessionDB, we can probably ignore this error");
                return context.Response.WriteAsync("Timeout encountered during login");
            }

            // Redirect to login if login session has timed out
            if (ex.Message == "An error was encountered while handling the remote login." ||
                (ex.Message == "Exception occurred while processing message."
                    && ex.InnerException.Message.Contains("IDX21324: The 'nonce' has expired", StringComparison.InvariantCultureIgnoreCase)))
            {
                _logger.LogWarning(ex, "Timeout exception during login");
                context.Response.StatusCode = 302;
                context.Response.Headers.Add("Location", "/");
                return context.Response.WriteAsync("Timeout encountered during login");
            }

            else
            {
                _logger.LogError(ex, "Unhandled exception encountered");
                var errorMessage = JsonConvert.SerializeObject(new { Message = ex.Message, Code = "GE" });
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return context.Response.WriteAsync(errorMessage);
            }

        }
    }
}
