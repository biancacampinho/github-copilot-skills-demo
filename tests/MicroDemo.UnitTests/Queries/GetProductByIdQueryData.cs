using MicroDemo.Application.Queries.Products;

namespace MicroDemo.UnitTests.Queries;

/// <summary>Test-data variants of <see cref="GetProductByIdQuery"/> for handler tests.</summary>
public static class GetProductByIdQueryData
{
    public static GetProductByIdQuery ForId(Guid id) => new(id);

    public static GetProductByIdQuery Unknown() => new(Guid.NewGuid());
}
