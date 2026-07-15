using SkillGhcDemo.Domain.Entities;
using SkillGhcDemo.Domain.Enums;

namespace SkillGhcDemo.Application.Dtos;

/// <summary>
/// Read projection of an <see cref="OrderItem"/> for a given product: order-level data,
/// the line itself, the buying <see cref="User"/> and the <see cref="Product"/> details.
/// </summary>
public class OrderByProductDto
{
    public Guid OrderId { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime OrderDateUtc { get; set; }

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string Currency { get; set; } = string.Empty;
    public decimal LineTotal { get; set; }

    public UserDto User { get; set; } = null!;
    public ProductDto Product { get; set; } = null!;
}

public static class OrderByProductMappingExtensions
{
    public static OrderByProductDto ToOrderByProductDto(this OrderItem item) => new()
    {
        OrderId = item.OrderId,
        Status = item.Order.Status,
        OrderDateUtc = item.Order.OrderDateUtc,
        Quantity = item.Quantity,
        UnitPrice = item.UnitPrice,
        Currency = item.Currency,
        LineTotal = item.LineTotal,
        User = item.Order.User.ToDto(),
        Product = item.Product.ToDto()
    };
}
