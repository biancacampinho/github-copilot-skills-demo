using MicroDemo.Api.Common;
using MicroDemo.Application.Features.Utenti.Commands.CreateUtente;
using MicroDemo.Application.Features.Utenti.Commands.DeleteUtente;
using MicroDemo.Application.Features.Utenti.Commands.UpdateUtente;
using MicroDemo.Application.Features.Utenti.Dtos;
using MicroDemo.Application.Features.Utenti.Queries.GetUtenteById;
using MicroDemo.Application.Features.Utenti.Queries.GetUtenti;
using Microsoft.AspNetCore.Mvc;

namespace MicroDemo.Api.Controllers;

public class UtentiController : ApiControllerBase
{
    /// <summary>Lista utenti, com filtro opcional de texto.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<UtenteDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] string? search = null, [FromQuery] bool onlyActive = false)
        => ToResponse(await Mediator.Send(new GetUtentiQuery(search, onlyActive)));

    /// <summary>Obtém um utente pelo id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UtenteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
        => ToResponse(await Mediator.Send(new GetUtenteByIdQuery(id)));

    /// <summary>Cria um novo utente.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateUtenteCommand command)
    {
        var result = await Mediator.Send(command);
        return result.Succeeded
            ? CreatedAtAction(nameof(GetById), new { id = result.Data }, result.Data)
            : ToResponse(result);
    }

    /// <summary>Atualiza um utente existente.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUtenteCommand command)
    {
        if (id != command.Id)
            return BadRequest(new { error = "O id da rota difere do id do corpo." });

        return ToResponse(await Mediator.Send(command));
    }

    /// <summary>Remove um utente.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
        => ToResponse(await Mediator.Send(new DeleteUtenteCommand(id)));
}
