using MicroDemo.Api.Common;
using MicroDemo.Application.Features.Orders.Commands.CreateOrder;
using MicroDemo.Application.Features.Orders.Dtos;
using MicroDemo.Application.Features.Orders.Queries.GetOrderById;
using Microsoft.AspNetCore.Mvc;

namespace MicroDemo.Api.Controllers;

public class OrdersController : ApiControllerBase
{
    /// <summary>Obtém um pedido pelo id (inclui as linhas).</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
        => ToResponse(await Mediator.Send(new GetOrderByIdQuery(id)));

    /// <summary>
    /// ★ ENDPOINT RESERVADO PARA IMPLEMENTAÇÃO MANUAL (skill personalizada) ★
    /// Cria um pedido para um utilizador, com múltiplos itens, fazendo o snapshot
    /// do preço corrente de cada produto e calculando o total.
    /// O endpoint e o pipeline já estão prontos; o handler
    /// (CreateOrderCommandHandler) está como stub aguardando implementação.
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
