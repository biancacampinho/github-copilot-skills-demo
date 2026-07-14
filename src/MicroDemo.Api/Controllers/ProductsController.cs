using MicroDemo.Api.Common;
using MicroDemo.Application.Commands.Products;
using MicroDemo.Application.Dtos;
using MicroDemo.Application.Queries.Products;
using Microsoft.AspNetCore.Mvc;

namespace MicroDemo.Api.Controllers;

public class ProductsController : ApiControllerBase
{
    /// <summary>Lists products, with optional filters by text, category and status.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ProductDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search = null,
        [FromQuery] Guid? categoryId = null,
        [FromQuery] bool onlyActive = false)
        => ToResponse(await Mediator.Send(new GetProductsQuery(search, categoryId, onlyActive)));

    /// <summary>Gets a product by id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
        => ToResponse(await Mediator.Send(new GetProductByIdQuery(id)));

    /// <summary>Creates a new product.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
    {
        var result = await Mediator.Send(command);
        return result.Succeeded
            ? CreatedAtAction(nameof(GetById), new { id = result.Data }, result.Data)
            : ToResponse(result);
    }

    /// <summary>Updates an existing product.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductCommand command)
    {
        if (id != command.Id)
            return BadRequest(new { error = "The route id differs from the body id." });

        return ToResponse(await Mediator.Send(command));
    }

    /// <summary>Removes a product.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(Guid id)
        => ToResponse(await Mediator.Send(new DeleteProductCommand(id)));
}
