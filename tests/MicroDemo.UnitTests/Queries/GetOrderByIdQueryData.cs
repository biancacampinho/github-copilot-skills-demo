using MicroDemo.Application.Queries.Orders;

namespace MicroDemo.UnitTests.Queries;

/// <summary>Test-data variants of <see cref="GetOrderByIdQuery"/> for handler tests.</summary>
public static class GetOrderByIdQueryData
{
    public static GetOrderByIdQuery ForId(Guid id) => new(id);

    public static GetOrderByIdQuery Unknown() => new(Guid.NewGuid());
}
