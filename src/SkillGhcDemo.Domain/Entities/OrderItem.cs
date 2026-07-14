using SkillGhcDemo.Domain.Common;

namespace SkillGhcDemo.Domain.Entities;

/// <summary>
/// Line of an <see cref="Order"/>: links a <see cref="Product"/> to an order, with the
/// quantity and a <b>snapshot</b> of the unit price at the time of purchase (so that
/// future changes to the <see cref="Price"/> do not affect orders already placed).
/// </summary>
public class OrderItem : BaseEntity
{
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int Quantity { get; set; }

    /// <summary>Snapshot of the unit price at the time of purchase.</summary>
    public decimal UnitPrice { get; set; }

    public string Currency { get; set; } = "EUR";

    /// <summary>Line total = <see cref="UnitPrice"/> × <see cref="Quantity"/>.</summary>
    public decimal LineTotal { get; set; }
}
