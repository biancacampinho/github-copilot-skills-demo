using SkillGhcDemo.Application.Commands.Products;

namespace SkillGhcDemo.UnitTests.Commands;

/// <summary>Test-data variants of <see cref="CreateProductCommand"/> for handler and validator tests.</summary>
public static class CreateProductCommandData
{
    public static CreateProductCommand Valid(Guid? categoryId = null) => new()
    {
        Name = "Bluetooth headphones",
        Description = "Over-ear headphones with noise cancelling",
        Sku = "SKU-HEADPHONE-001",
        CategoryId = categoryId ?? Guid.NewGuid(),
        IsActive = true
    };

    public static CreateProductCommand WithEmptyName(Guid? categoryId = null) => Valid(categoryId) with { Name = "" };

    public static CreateProductCommand WithEmptySku(Guid? categoryId = null) => Valid(categoryId) with { Sku = "" };

    public static CreateProductCommand WithEmptyCategory() => Valid() with { CategoryId = Guid.Empty };
}
