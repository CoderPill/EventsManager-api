using EventsManager.Core.Constants;
using EventsManager.Infrastructure.Tools.Logging;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;

namespace EventsManager.Api.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IExceptionInfoExtractor _extractor;
        private readonly IExceptionLogStorage _logStorage;

        public GlobalExceptionMiddleware(RequestDelegate next, IExceptionInfoExtractor extractor, IExceptionLogStorage logStorage)
        {
            _next = next;
            _extractor = extractor;
            _logStorage = logStorage;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                try
                {
                    await _logStorage.WriteAsync(_extractor.ExtractExceptionInfo(ex));
                }
                catch (Exception logEx)
                {
                    Console.Error.WriteLine(logEx.Message);
                }
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var statusCode = exception switch
            {
                ArgumentNullException or ArgumentException => StatusCodes.Status400BadRequest,
                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                KeyNotFoundException => StatusCodes.Status404NotFound,
                InvalidOperationException => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError
            };

            context.Response.ContentType = SystemValues.Infrastructure.JsonContentType;
            context.Response.StatusCode = statusCode;

            var response = new
            {
                status = statusCode,
                Error = GetGenericMessage(statusCode)
            };

            await context.Response.WriteAsJsonAsync(response);
        }

        private static string GetGenericMessage(int statusCode) => statusCode switch
        {
            StatusCodes.Status400BadRequest => SystemMessages.Infrastructure.Error_BadRequest,
            StatusCodes.Status401Unauthorized => SystemMessages.Infrastructure.Error_Unauthorized,
            StatusCodes.Status404NotFound => SystemMessages.Infrastructure.Error_NotFound,
            StatusCodes.Status409Conflict => SystemMessages.Infrastructure.Error_Conflict,
            _ => SystemMessages.Infrastructure.Error_Internal
        };

    }
}
