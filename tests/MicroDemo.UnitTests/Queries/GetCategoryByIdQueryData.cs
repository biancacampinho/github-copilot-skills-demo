using MicroDemo.Application.Queries.Categories;

namespace MicroDemo.UnitTests.Queries;

/// <summary>Test-data variants of <see cref="GetCategoryByIdQuery"/> for handler tests.</summary>
public static class GetCategoryByIdQueryData
{
    public static GetCategoryByIdQuery ForId(Guid id) => new(id);

    public static GetCategoryByIdQuery Unknown() => new(Guid.NewGuid());
}
