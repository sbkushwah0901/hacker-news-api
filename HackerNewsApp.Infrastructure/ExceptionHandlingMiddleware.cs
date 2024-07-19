using HackerNewsApp.BusinessEntities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace HackerNewsApp.Infrastructure
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode status;
            string message;

            if (exception is CustomException customException)
            {
                status = customException.StatusCode;
                message = customException.Message;
            }
            else
            {
                status = HttpStatusCode.InternalServerError;
                message = "Internal Server Error from the custom middleware.";
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;
            var errorDetails = new ErrorDetailsModel
            {
                StatusCode = context.Response.StatusCode,
                Message = message
            };

            return context.Response.WriteAsync(errorDetails.ToString());
        }

    }
}
