using System.Net;
using System.Text.Json;

namespace Taskboard.Api.Middlewares
{
    public class ErrorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorMiddleware> _logger;

        public ErrorMiddleware(RequestDelegate next, ILogger<ErrorMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred");

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    error = ex.Message,
                    path = context.Request.Path,
                    time = DateTime.UtcNow
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }

    public static class ErrorHandlingExtensions
    {
        public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ErrorMiddleware>();
        }
    }
}
