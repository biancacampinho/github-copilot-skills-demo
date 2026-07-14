using MicroDemo.Application.Commands.Products;

namespace MicroDemo.UnitTests.Commands;

/// <summary>Test-data variants of <see cref="UpdateProductCommand"/> for handler and validator tests.</summary>
public static class UpdateProductCommandData
{
    public static UpdateProductCommand Valid(Guid id, Guid? categoryId = null) => new()
    {
        Id = id,
        Name = "Bluetooth headphones",
        Description = "Over-ear headphones with noise cancelling",
        Sku = "SKU-HEADPHONE-001",
        CategoryId = categoryId ?? Guid.NewGuid(),
        IsActive = true
    };

    public static UpdateProductCommand WithEmptyName(Guid id, Guid? categoryId = null) => Valid(id, categoryId) with { Name = "" };

    public static UpdateProductCommand WithEmptyId() => Valid(Guid.Empty);
}
