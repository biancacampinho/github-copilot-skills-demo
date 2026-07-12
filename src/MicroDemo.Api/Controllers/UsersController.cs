using MicroDemo.Api.Common;
using MicroDemo.Application.Features.Users.Commands.CreateUser;
using MicroDemo.Application.Features.Users.Commands.DeleteUser;
using MicroDemo.Application.Features.Users.Commands.UpdateUser;
using MicroDemo.Application.Features.Users.Dtos;
using MicroDemo.Application.Features.Users.Queries.GetUserById;
using MicroDemo.Application.Features.Users.Queries.GetUsers;
using Microsoft.AspNetCore.Mvc;

namespace MicroDemo.Api.Controllers;

public class UsersController : ApiControllerBase
{
    /// <summary>Lista utilizadores, com filtro opcional de texto.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<UserDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] string? search = null, [FromQuery] bool onlyActive = false)
        => ToResponse(await Mediator.Send(new GetUsersQuery(search, onlyActive)));

    /// <summary>Obtém um utilizador pelo id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
        => ToResponse(await Mediator.Send(new GetUserByIdQuery(id)));

    /// <summary>Cria um novo utilizador.</summary>
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

    /// <summary>Atualiza um utilizador existente.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserCommand command)
    {
        if (id != command.Id)
            return BadRequest(new { error = "O id da rota difere do id do corpo." });

        return ToResponse(await Mediator.Send(command));
    }

    /// <summary>Remove um utilizador.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(Guid id)
        => ToResponse(await Mediator.Send(new DeleteUserCommand(id)));
}
