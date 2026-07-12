using MediatR;
using MicroDemo.Application.Common.Models;

namespace MicroDemo.Application.Features.Subscriptions.Commands.CreateSubscription;

/// <summary>
/// Cria uma assinatura associando um <c>Utente</c> a um <c>Price</c>.
///
/// ┌──────────────────────────────────────────────────────────────────────────┐
/// │  ★ ENDPOINT RESERVADO PARA IMPLEMENTAÇÃO MANUAL (via skill personalizada)  │
/// │  Ver <see cref="CreateSubscriptionCommandHandler"/> — o handler está como   │
/// │  stub. Command, Validator e endpoint no controller já estão prontos.       │
/// └──────────────────────────────────────────────────────────────────────────┘
/// </summary>
public record CreateSubscriptionCommand : IRequest<Result<Guid>>
{
    public Guid UtenteId { get; init; }
    public Guid PriceId { get; init; }
    public DateTime StartDateUtc { get; init; }
    public DateTime? EndDateUtc { get; init; }
}
