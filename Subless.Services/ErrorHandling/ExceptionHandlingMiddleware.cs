using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Subless.Services.ErrorHandling
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
            try
            {
                var requestUri = context?.Request?.GetEncodedPathAndQuery();
                _logger.LogInformation($"Exception occurred on reuqest to {requestUri}");
            }
            catch(Exception e)
            {
                _logger.LogWarning(e, "Could not capture request details");
            }
            try
            {
                foreach (var claim in context.User.Claims)
                {
                    _logger.LogInformation($"Claim of user encountering exception: {claim.Type} = {claim.Value}");
                }
            }
            catch(Exception e)
            {
                _logger.LogWarning(e, "Could not capture claim details");
            }
            try
            {
                foreach (var header in context.Request.Headers)
                {
                    _logger.LogInformation($"Header information of request encountering exception: {header.Key} = {header.Value}");
                }
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Could not capture header details");
            }
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

            if (ex is BadHttpRequestException && context.Request.Path.Value.Contains("Hit", StringComparison.InvariantCultureIgnoreCase))
            {
                _logger.LogWarning(ex, "Exception occurred when processing a hit");
                context.Response.StatusCode = 400;
                return context.Response.WriteAsync("Hit could not be processed");
            }

            // Redirect to login if login session has timed out
            if (ex.Message == "An error was encountered while handling the remote login." ||
                ex.Message == "Exception occurred while processing message."
                    && ex.InnerException.Message.Contains("IDX21324: The 'nonce' has expired", StringComparison.InvariantCultureIgnoreCase))
            {
                _logger.LogWarning(ex, "Timeout exception during login");
                context.Response.StatusCode = 302;
                context.Response.Headers.Add("Location", "/");
                return context.Response.WriteAsync("Timeout encountered during login");
            }

            if (ex is BadHttpRequestException && ex.Message == "Unexpected end of request content.")
            {
                _logger.LogWarning(ex, "Partial request received");
                context.Response.StatusCode = 400;
                return context.Response.WriteAsync("Partial request received");
            }
            else
            {
                _logger.LogError(ex, "Unhandled exception encountered");
                var errorMessage = JsonConvert.SerializeObject(new { ex.Message, Code = "GE" });
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return context.Response.WriteAsync(errorMessage);
            }

        }
    }
}
