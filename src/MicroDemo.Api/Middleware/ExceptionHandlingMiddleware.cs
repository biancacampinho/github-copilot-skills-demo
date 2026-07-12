using System.Net;
using System.Text.Json;
using MicroDemo.Application.Common.Exceptions;

namespace MicroDemo.Api.Middleware;

/// <summary>
/// Captura exceções não tratadas e as converte num payload JSON consistente.
/// Trata especialmente a <see cref="ValidationException"/> lançada pelo
/// ValidationBehavior do MediatR (→ HTTP 400).
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
            _logger.LogWarning("Falha de validação: {Errors}", string.Join("; ", ex.FlattenedErrors));
            await WriteAsync(context, HttpStatusCode.BadRequest, new
            {
                error = ex.Message,
                errors = ex.Errors
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro não tratado ao processar {Path}", context.Request.Path);
            await WriteAsync(context, HttpStatusCode.InternalServerError, new
            {
                error = "Ocorreu um erro inesperado."
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
