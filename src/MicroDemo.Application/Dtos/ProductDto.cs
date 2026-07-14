using MicroDemo.Domain.Entities;

namespace MicroDemo.Application.Dtos;

/// <summary>Read projection of <see cref="Product"/>.</summary>
public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Sku { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public Guid CategoryId { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    /// <summary>Current unit price (most recent active), if any.</summary>
    public decimal? CurrentPrice { get; set; }
    public string? Currency { get; set; }
}

public static class ProductMappingExtensions
{
    public static ProductDto ToDto(this Product product)
    {
        var current = product.Prices
            .Where(p => p.IsActive)
            .OrderByDescending(p => p.ValidFromUtc)
            .FirstOrDefault();

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Sku = product.Sku,
            IsActive = product.IsActive,
            CategoryId = product.CategoryId,
            CreatedAtUtc = product.CreatedAtUtc,
            CurrentPrice = current?.Amount,
            Currency = current?.Currency
        };
    }
}
