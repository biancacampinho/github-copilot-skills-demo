using MicroDemo.Api.Common;
using MicroDemo.Application.Features.Categories.Commands.CreateCategory;
using MicroDemo.Application.Features.Categories.Commands.DeleteCategory;
using MicroDemo.Application.Features.Categories.Commands.UpdateCategory;
using MicroDemo.Application.Features.Categories.Dtos;
using MicroDemo.Application.Features.Categories.Queries.GetCategories;
using MicroDemo.Application.Features.Categories.Queries.GetCategoryById;
using Microsoft.AspNetCore.Mvc;

namespace MicroDemo.Api.Controllers;

public class CategoriesController : ApiControllerBase
{
    /// <summary>Lista categorias.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CategoryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] bool onlyActive = false)
        => ToResponse(await Mediator.Send(new GetCategoriesQuery(onlyActive)));

    /// <summary>Obtém uma categoria pelo id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
        => ToResponse(await Mediator.Send(new GetCategoryByIdQuery(id)));

    /// <summary>Cria uma nova categoria.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateCategoryCommand command)
    {
        var result = await Mediator.Send(command);
        return result.Succeeded
            ? CreatedAtAction(nameof(GetById), new { id = result.Data }, result.Data)
            : ToResponse(result);
    }

    /// <summary>Atualiza uma categoria existente.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryCommand command)
    {
        if (id != command.Id)
            return BadRequest(new { error = "O id da rota difere do id do corpo." });

        return ToResponse(await Mediator.Send(command));
    }

    /// <summary>Remove uma categoria.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(Guid id)
        => ToResponse(await Mediator.Send(new DeleteCategoryCommand(id)));
}
