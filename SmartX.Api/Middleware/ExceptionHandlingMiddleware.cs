using System.Net;
using System.Text.Json;

namespace SmartX.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            _logger.LogError(ex, "Unhandled exception processing {Path}", context.Request.Path);

            var statusCode = ex switch
            {
                ArgumentException => HttpStatusCode.BadRequest,
                KeyNotFoundException => HttpStatusCode.NotFound,
                InvalidOperationException => HttpStatusCode.Conflict,
                _ => HttpStatusCode.InternalServerError
            };

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = (int)statusCode;

            var problem = new
            {
                title = ex.GetType().Name,
                status = (int)statusCode,
                detail = ex.Message,
                instance = context.Request.Path.Value
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
        }
    }
}