using MediatR;
using MicroDemo.Application.Common.Models;

namespace MicroDemo.Application.Commands.Orders;

/// <summary>
/// Creates an order (<c>Order</c>) for a <c>User</c>, with one or more lines
/// (<c>OrderItem</c>), taking a snapshot of the current price of each product and
/// calculating the total.
///
/// ┌──────────────────────────────────────────────────────────────────────────┐
/// │  ★ ENDPOINT RESERVED FOR MANUAL IMPLEMENTATION (via custom skill)          │
/// │  See <see cref="CreateOrderCommandHandler"/> — the handler is a stub.       │
/// │  Command, Validator and controller endpoint are already ready.             │
/// └──────────────────────────────────────────────────────────────────────────┘
/// </summary>
public record CreateOrderCommand : IRequest<Result<Guid>>
{
    public Guid UserId { get; init; }
    public IReadOnlyList<CreateOrderItem> Items { get; init; } = new List<CreateOrderItem>();
}

/// <summary>Line requested in the order: which product and how many units.</summary>
public record CreateOrderItem
{
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
}
