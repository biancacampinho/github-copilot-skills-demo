using MediatR;
using SkillGhcDemo.Application.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace SkillGhcDemo.Api.Common;

/// <summary>
/// Base class for controllers. Exposes the MediatR <see cref="ISender"/> and converts
/// the <see cref="Result"/> envelope into consistent HTTP responses.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class ApiControllerBase : ControllerBase
{
    private ISender? _mediator;
    protected ISender Mediator =>
        _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    /// <summary>Converts a <see cref="Result{T}"/> into an <see cref="IActionResult"/>.</summary>
    protected IActionResult ToResponse<T>(Result<T> result) =>
        result.Succeeded
            ? Ok(result.Data)
            : MapError(result);

    /// <summary>Converts a <see cref="Result"/> (without payload) into an <see cref="IActionResult"/>.</summary>
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
