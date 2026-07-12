using MediatR;
using MicroDemo.Application.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace MicroDemo.Api.Common;

/// <summary>
/// Base para os controllers. Expõe o <see cref="ISender"/> do MediatR e converte
/// o envelope <see cref="Result"/> em respostas HTTP consistentes.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class ApiControllerBase : ControllerBase
{
    private ISender? _mediator;
    protected ISender Mediator =>
        _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    /// <summary>Converte um <see cref="Result{T}"/> em <see cref="IActionResult"/>.</summary>
    protected IActionResult ToResponse<T>(Result<T> result) =>
        result.Succeeded
            ? Ok(result.Data)
            : MapError(result);

    /// <summary>Converte um <see cref="Result"/> (sem payload) em <see cref="IActionResult"/>.</summary>
    protected IActionResult ToResponse(Result result) =>
        result.Succeeded
            ? NoContent()
            : MapError(result);

    private IActionResult MapError(Result result) => result.ErrorType switch
    {
        ResultErrorType.NotFound => NotFound(new { error = result.Error }),
        ResultErrorType.Conflict => Conflict(new { error = result.Error }),
        ResultErrorType.Validation => BadRequest(new { error = result.Error, errors = result.ValidationErrors }),
        _ => BadRequest(new { error = result.Error })
    };
}
