using MicroDemo.Application.Commands.Categories;

namespace MicroDemo.UnitTests.Commands;

/// <summary>Test-data variants of <see cref="DeleteCategoryCommand"/> for handler tests.</summary>
public static class DeleteCategoryCommandData
{
    public static DeleteCategoryCommand ForId(Guid id) => new(id);

    public static DeleteCategoryCommand Unknown() => new(Guid.NewGuid());
}
