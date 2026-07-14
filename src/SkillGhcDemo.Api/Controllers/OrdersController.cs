using SkillGhcDemo.Api.Common;
using SkillGhcDemo.Application.Commands.Orders;
using SkillGhcDemo.Application.Dtos;
using SkillGhcDemo.Application.Queries.Orders;
using Microsoft.AspNetCore.Mvc;

namespace SkillGhcDemo.Api.Controllers;

public class OrdersController : ApiControllerBase
{
    /// <summary>Gets an order by id (includes its lines).</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
        => ToResponse(await Mediator.Send(new GetOrderByIdQuery(id)));

    /// <summary>
    /// ★ ENDPOINT RESERVED FOR MANUAL IMPLEMENTATION (custom skill) ★
    /// Creates an order for a user, with multiple items, taking a snapshot
    /// of the current price of each product and computing the total.
    /// The endpoint and the pipeline are already in place; the handler
    /// (CreateOrderCommandHandler) is a stub awaiting implementation.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateOrderCommand command)
    {
        var result = await Mediator.Send(command);
        return result.Succeeded
            ? CreatedAtAction(nameof(GetById), new { id = result.Data }, result.Data)
            : ToResponse(result);
    }
}
