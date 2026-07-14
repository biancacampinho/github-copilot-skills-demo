using MicroDemo.Application.Queries.Products;

namespace MicroDemo.UnitTests.Queries;

/// <summary>Test-data variants of <see cref="GetProductsQuery"/> for handler tests.</summary>
public static class GetProductsQueryData
{
    public static GetProductsQuery All() => new();

    public static GetProductsQuery OnlyActive() => new(OnlyActive: true);

    public static GetProductsQuery ByCategory(Guid categoryId) => new(CategoryId: categoryId);

    public static GetProductsQuery Search(string search) => new(Search: search);
}
