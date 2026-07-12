using MicroDemo.Api.Common;
using MicroDemo.Application.Features.Subscriptions.Commands.CreateSubscription;
using MicroDemo.Application.Features.Subscriptions.Dtos;
using MicroDemo.Application.Features.Subscriptions.Queries.GetSubscriptionById;
using Microsoft.AspNetCore.Mvc;

namespace MicroDemo.Api.Controllers;

public class SubscriptionsController : ApiControllerBase
{
    /// <summary>Obtém uma assinatura pelo id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SubscriptionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
        => ToResponse(await Mediator.Send(new GetSubscriptionByIdQuery(id)));

    /// <summary>
    /// ★ ENDPOINT RESERVADO PARA IMPLEMENTAÇÃO MANUAL (skill personalizada) ★
    /// Cria uma assinatura associando um utente a um price.
    /// O endpoint e o pipeline já estão prontos; o handler
    /// (CreateSubscriptionCommandHandler) está como stub aguardando implementação.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateSubscriptionCommand command)
    {
        var result = await Mediator.Send(command);
        return result.Succeeded
            ? CreatedAtAction(nameof(GetById), new { id = result.Data }, result.Data)
            : ToResponse(result);
    }
}
