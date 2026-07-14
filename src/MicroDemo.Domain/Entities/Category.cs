using MicroDemo.Domain.Common;

namespace MicroDemo.Domain.Entities;

/// <summary>
/// Product category (e.g., "Electronics", "Books"). A product belongs to one category.
/// </summary>
public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
