using MicroDemo.Application.Commands.Categories;

namespace MicroDemo.UnitTests.Commands;

/// <summary>Test-data variants of <see cref="CreateCategoryCommand"/> for handler and validator tests.</summary>
public static class CreateCategoryCommandData
{
    public static CreateCategoryCommand Valid() => new()
    {
        Name = "Electronics",
        Description = "Electronic devices and accessories",
        IsActive = true
    };

    public static CreateCategoryCommand WithName(string name) => Valid() with { Name = name };

    public static CreateCategoryCommand WithEmptyName() => Valid() with { Name = "" };

    public static CreateCategoryCommand WithTooLongName() => Valid() with { Name = new string('A', 121) };

    public static CreateCategoryCommand Inactive() => Valid() with { IsActive = false };
}
