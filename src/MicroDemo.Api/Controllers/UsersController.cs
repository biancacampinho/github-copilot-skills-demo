using MicroDemo.Api.Common;
using MicroDemo.Application.Commands.Users;
using MicroDemo.Application.Dtos;
using MicroDemo.Application.Queries.Users;
using Microsoft.AspNetCore.Mvc;

namespace MicroDemo.Api.Controllers;

public class UsersController : ApiControllerBase
{
    /// <summary>Lists users, with an optional text filter.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<UserDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] string? search = null, [FromQuery] bool onlyActive = false)
        => ToResponse(await Mediator.Send(new GetUsersQuery(search, onlyActive)));

    /// <summary>Gets a user by id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
        => ToResponse(await Mediator.Send(new GetUserByIdQuery(id)));

    /// <summary>Creates a new user.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
    {
        var result = await Mediator.Send(command);
        return result.Succeeded
            ? CreatedAtAction(nameof(GetById), new { id = result.Data }, result.Data)
            : ToResponse(result);
    }

    /// <summary>Updates an existing user.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserCommand command)
    {
        if (id != command.Id)
            return BadRequest(new { error = "The route id differs from the body id." });

        return ToResponse(await Mediator.Send(command));
    }

    /// <summary>Removes a user.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(Guid id)
        => ToResponse(await Mediator.Send(new DeleteUserCommand(id)));
}
