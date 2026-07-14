using MicroDemo.Api.Common;
using MicroDemo.Application.Commands.Prices;
using MicroDemo.Application.Dtos;
using MicroDemo.Application.Queries.Prices;
using Microsoft.AspNetCore.Mvc;

namespace MicroDemo.Api.Controllers;

public class PricesController : ApiControllerBase
{
    /// <summary>Lists prices, with optional filters by product and status.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<PriceDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] Guid? productId = null, [FromQuery] bool onlyActive = false)
        => ToResponse(await Mediator.Send(new GetPricesQuery(productId, onlyActive)));

    /// <summary>Gets a price by id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PriceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
        => ToResponse(await Mediator.Send(new GetPriceByIdQuery(id)));

    /// <summary>Creates a new price for a product.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreatePriceCommand command)
    {
        var result = await Mediator.Send(command);
        return result.Succeeded
            ? CreatedAtAction(nameof(GetById), new { id = result.Data }, result.Data)
            : ToResponse(result);
    }

    /// <summary>Updates an existing price.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePriceCommand command)
    {
        if (id != command.Id)
            return BadRequest(new { error = "The route id differs from the body id." });

        return ToResponse(await Mediator.Send(command));
    }

    /// <summary>Removes a price.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
        => ToResponse(await Mediator.Send(new DeletePriceCommand(id)));
}
