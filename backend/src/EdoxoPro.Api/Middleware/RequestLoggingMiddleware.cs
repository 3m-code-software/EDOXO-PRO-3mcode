using System.Diagnostics;
using Serilog;

namespace EdoxoPro.Api.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();
        var method = context.Request.Method;
        var path = context.Request.Path;

        await _next(context);

        sw.Stop();
        var statusCode = context.Response.StatusCode;
        var elapsed = sw.ElapsedMilliseconds;

        Log.Information("HTTP {Method} {Path} responded {StatusCode} in {Elapsed}ms",
            method, path, statusCode, elapsed);
    }
}
