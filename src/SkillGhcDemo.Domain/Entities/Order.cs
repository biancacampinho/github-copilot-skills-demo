using SkillGhcDemo.Domain.Common;
using SkillGhcDemo.Domain.Enums;

namespace SkillGhcDemo.Domain.Entities;

/// <summary>
/// A purchase order placed by a <see cref="User"/>. Aggregates one or more
/// <see cref="OrderItem"/> and keeps the total calculated at the time of purchase.
/// </summary>
public class Order : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    /// <summary>Sum of the <see cref="OrderItem.LineTotal"/> values at the time of purchase.</summary>
    public decimal TotalAmount { get; set; }

    public string Currency { get; set; } = "EUR";

    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public DateTime OrderDateUtc { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
