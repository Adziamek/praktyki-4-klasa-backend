using System.Net;
using System.Text.Json;

namespace Warehouse.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unhandled exception for {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title) = exception switch
        {
            KeyNotFoundException => (HttpStatusCode.NotFound, "Resource not found"),
            UnauthorizedAccessException => (HttpStatusCode.Forbidden, "Access denied"),
            ArgumentException => (HttpStatusCode.BadRequest, "Invalid request"),
            InvalidOperationException => (HttpStatusCode.BadRequest, "Invalid operation"),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred")
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;

        var problem = new
        {
            type = $"https://httpstatuses.com/{(int)statusCode}",
            title,
            status = (int)statusCode,
            detail = _env.IsDevelopment() ? exception.Message : "Please contact support if the problem persists.",
            traceId = context.TraceIdentifier
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
    }
}