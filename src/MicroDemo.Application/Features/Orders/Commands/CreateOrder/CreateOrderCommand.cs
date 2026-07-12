using MediatR;
using MicroDemo.Application.Common.Models;

namespace MicroDemo.Application.Features.Orders.Commands.CreateOrder;

/// <summary>
/// Cria um pedido (<c>Order</c>) para um <c>User</c>, com uma ou mais linhas
/// (<c>OrderItem</c>), fazendo o snapshot do preço corrente de cada produto e
/// calculando o total.
///
/// ┌──────────────────────────────────────────────────────────────────────────┐
/// │  ★ ENDPOINT RESERVADO PARA IMPLEMENTAÇÃO MANUAL (via skill personalizada)  │
/// │  Ver <see cref="CreateOrderCommandHandler"/> — o handler está como stub.    │
/// │  Command, Validator e endpoint no controller já estão prontos.             │
/// └──────────────────────────────────────────────────────────────────────────┘
/// </summary>
public record CreateOrderCommand : IRequest<Result<Guid>>
{
    public Guid UserId { get; init; }
    public IReadOnlyList<CreateOrderItem> Items { get; init; } = new List<CreateOrderItem>();
}

/// <summary>Linha solicitada no pedido: qual produto e quantas unidades.</summary>
public record CreateOrderItem
{
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
}
