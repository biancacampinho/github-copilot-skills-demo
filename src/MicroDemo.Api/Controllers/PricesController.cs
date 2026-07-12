using MicroDemo.Api.Common;
using MicroDemo.Application.Features.Prices.Commands.CreatePrice;
using MicroDemo.Application.Features.Prices.Commands.DeletePrice;
using MicroDemo.Application.Features.Prices.Commands.UpdatePrice;
using MicroDemo.Application.Features.Prices.Dtos;
using MicroDemo.Application.Features.Prices.Queries.GetPriceById;
using MicroDemo.Application.Features.Prices.Queries.GetPrices;
using Microsoft.AspNetCore.Mvc;

namespace MicroDemo.Api.Controllers;

public class PricesController : ApiControllerBase
{
    /// <summary>Lista todos os preços.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<PriceDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] bool onlyActive = false)
        => ToResponse(await Mediator.Send(new GetPricesQuery(onlyActive)));

    /// <summary>Obtém um preço pelo id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PriceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
        => ToResponse(await Mediator.Send(new GetPriceByIdQuery(id)));

    /// <summary>Cria um novo preço.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreatePriceCommand command)
    {
        var result = await Mediator.Send(command);
        return result.Succeeded
            ? CreatedAtAction(nameof(GetById), new { id = result.Data }, result.Data)
            : ToResponse(result);
    }

    /// <summary>Atualiza um preço existente.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePriceCommand command)
    {
        if (id != command.Id)
            return BadRequest(new { error = "O id da rota difere do id do corpo." });

        return ToResponse(await Mediator.Send(command));
    }

    /// <summary>Remove um preço.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(Guid id)
        => ToResponse(await Mediator.Send(new DeletePriceCommand(id)));
}
