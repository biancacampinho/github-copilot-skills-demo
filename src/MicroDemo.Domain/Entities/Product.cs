using MicroDemo.Domain.Common;

namespace MicroDemo.Domain.Entities;

/// <summary>
/// A sellable product. Belongs to a <see cref="Category"/> and has a price
/// history (<see cref="Price"/>). Appears in orders via <see cref="OrderItem"/>.
/// </summary>
public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    /// <summary>Unique product code (Stock Keeping Unit).</summary>
    public string Sku { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    // Navigation
    /// <summary>Price history of the product (the "current" price is the most recent active one).</summary>
    public ICollection<Price> Prices { get; set; } = new List<Price>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
