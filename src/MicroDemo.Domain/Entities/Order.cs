using MicroDemo.Domain.Common;
using MicroDemo.Domain.Enums;

namespace MicroDemo.Domain.Entities;

/// <summary>
/// Pedido de compra realizado por um <see cref="User"/>. Agrega um ou mais
/// <see cref="OrderItem"/> e mantém o total calculado no momento da compra.
/// </summary>
public class Order : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    /// <summary>Soma dos <see cref="OrderItem.LineTotal"/> no momento da compra.</summary>
    public decimal TotalAmount { get; set; }

    public string Currency { get; set; } = "EUR";

    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public DateTime OrderDateUtc { get; set; } = DateTime.UtcNow;

    // Navegação
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
