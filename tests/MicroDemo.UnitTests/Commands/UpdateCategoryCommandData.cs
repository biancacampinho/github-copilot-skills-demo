using MicroDemo.Application.Commands.Categories;

namespace MicroDemo.UnitTests.Commands;

/// <summary>Test-data variants of <see cref="UpdateCategoryCommand"/> for handler and validator tests.</summary>
public static class UpdateCategoryCommandData
{
    public static UpdateCategoryCommand Valid(Guid id) => new()
    {
        Id = id,
        Name = "Electronics",
        Description = "Electronic devices and accessories",
        IsActive = true
    };

    public static UpdateCategoryCommand WithName(Guid id, string name) => Valid(id) with { Name = name };

    public static UpdateCategoryCommand WithEmptyName(Guid id) => Valid(id) with { Name = "" };

    public static UpdateCategoryCommand WithEmptyId() => Valid(Guid.Empty);
}
