using MicroDemo.Domain.Entities;
using MicroDemo.Domain.Enums;

namespace MicroDemo.Application.Dtos;

/// <summary>Read projection of <see cref="Order"/>, including its lines.</summary>
public class OrderDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public OrderStatus Status { get; set; }
    public DateTime OrderDateUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public IReadOnlyList<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
}

/// <summary>Read projection of an order line (<see cref="OrderItem"/>).</summary>
public class OrderItemDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string Currency { get; set; } = string.Empty;
    public decimal LineTotal { get; set; }
}

public static class OrderMappingExtensions
{
    public static OrderItemDto ToDto(this OrderItem item) => new()
    {
        Id = item.Id,
        ProductId = item.ProductId,
        Quantity = item.Quantity,
        UnitPrice = item.UnitPrice,
        Currency = item.Currency,
        LineTotal = item.LineTotal
    };

    public static OrderDto ToDto(this Order order) => new()
    {
        Id = order.Id,
        UserId = order.UserId,
        TotalAmount = order.TotalAmount,
        Currency = order.Currency,
        Status = order.Status,
        OrderDateUtc = order.OrderDateUtc,
        CreatedAtUtc = order.CreatedAtUtc,
        Items = order.Items.Select(i => i.ToDto()).ToList()
    };
}
