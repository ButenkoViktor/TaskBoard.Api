using System.Diagnostics;

namespace Taskboard.Api.Middlewares
{
    // Add my custom middleware to log request and response details RequestLoggingMiddleware
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            Console.WriteLine($"➡️ [{DateTime.Now:T}] {context.Request.Method} {context.Request.Path}");

            await _next(context); // передаем управление дальше

            stopwatch.Stop();
            Console.WriteLine($"⬅️ [{DateTime.Now:T}] {context.Response.StatusCode} ({stopwatch.ElapsedMilliseconds} ms)");
        }
    }

    public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}