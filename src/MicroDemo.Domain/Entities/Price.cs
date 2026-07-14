using MicroDemo.Domain.Common;

namespace MicroDemo.Domain.Entities;

/// <summary>
/// Price of a <see cref="Product"/>. Modeled as a history (1:N with Product):
/// multiple records over time, where the "current" price is the active one with the
/// most recent <see cref="ValidFromUtc"/>.
/// </summary>
public class Price : BaseEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public decimal Amount { get; set; }

    /// <summary>ISO-4217 code (e.g., "EUR", "BRL", "USD").</summary>
    public string Currency { get; set; } = "EUR";

    /// <summary>The point in time from which this price takes effect.</summary>
    public DateTime ValidFromUtc { get; set; } = DateTime.UtcNow;

    /// <summary>Indicates whether this is a currently effective price (not discontinued).</summary>
    public bool IsActive { get; set; } = true;
}
