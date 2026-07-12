using MicroDemo.Api.Common;
using MicroDemo.Application.Features.Products.Commands.CreateProduct;
using MicroDemo.Application.Features.Products.Commands.DeleteProduct;
using MicroDemo.Application.Features.Products.Commands.UpdateProduct;
using MicroDemo.Application.Features.Products.Dtos;
using MicroDemo.Application.Features.Products.Queries.GetProductById;
using MicroDemo.Application.Features.Products.Queries.GetProducts;
using Microsoft.AspNetCore.Mvc;

namespace MicroDemo.Api.Controllers;

public class ProductsController : ApiControllerBase
{
    /// <summary>Lista produtos, com filtros opcionais de texto, categoria e estado.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ProductDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search = null,
        [FromQuery] Guid? categoryId = null,
        [FromQuery] bool onlyActive = false)
        => ToResponse(await Mediator.Send(new GetProductsQuery(search, categoryId, onlyActive)));

    /// <summary>Obtém um produto pelo id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
        => ToResponse(await Mediator.Send(new GetProductByIdQuery(id)));

    /// <summary>Cria um novo produto.</summary>
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

    /// <summary>Atualiza um produto existente.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductCommand command)
    {
        if (id != command.Id)
            return BadRequest(new { error = "O id da rota difere do id do corpo." });

        return ToResponse(await Mediator.Send(command));
    }

    /// <summary>Remove um produto.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(Guid id)
        => ToResponse(await Mediator.Send(new DeleteProductCommand(id)));
}
