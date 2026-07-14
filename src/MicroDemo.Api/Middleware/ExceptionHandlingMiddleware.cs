using System.Net;
using System.Text.Json;
using MicroDemo.Application.Common.Exceptions;

namespace MicroDemo.Api.Middleware;

/// <summary>
/// Captures unhandled exceptions and converts them into a consistent JSON payload.
/// Specifically handles the <see cref="ValidationException"/> thrown by the
/// MediatR ValidationBehavior (→ HTTP 400).
/// </summary>
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
        catch (ValidationException ex)
        {
            _logger.LogWarning("Validation failure: {Errors}", string.Join("; ", ex.FlattenedErrors));
            await WriteAsync(context, HttpStatusCode.BadRequest, new
            {
                error = ex.Message,
                errors = ex.Errors
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error while processing {Path}", context.Request.Path);
            await WriteAsync(context, HttpStatusCode.InternalServerError, new
            {
                error = "An unexpected error occurred."
            });
        }
    }

    private static async Task WriteAsync(HttpContext context, HttpStatusCode status, object payload)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)status;
        await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
    }
}
