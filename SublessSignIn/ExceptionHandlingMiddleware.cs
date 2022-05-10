﻿using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;

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
            // Redirect to login if login session has timed out
            if (ex.Message == "An error was encountered while handling the remote login.")
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
