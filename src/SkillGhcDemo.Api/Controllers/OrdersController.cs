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

    /// <summary>Lists the orders that contain the given product, with full user and product details.</summary>
    [HttpGet("product/{productId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyList<OrderWithDetailsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByProductId(Guid productId)
        => ToResponse(await Mediator.Send(new GetOrdersByProductIdQuery(productId)));

    /// <summary>
    /// Creates an order for a user, with multiple items, taking a snapshot
    /// of the current price of each product and computing the total.
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
