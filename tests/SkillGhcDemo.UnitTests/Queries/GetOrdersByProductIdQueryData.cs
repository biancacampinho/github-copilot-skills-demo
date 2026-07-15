using SkillGhcDemo.Application.Queries.Orders;

namespace SkillGhcDemo.UnitTests.Queries;

/// <summary>Test-data variants of <see cref="GetOrdersByProductIdQuery"/> for handler tests.</summary>
public static class GetOrdersByProductIdQueryData
{
    public static GetOrdersByProductIdQuery ForProduct(Guid productId) => new(productId);

    public static GetOrdersByProductIdQuery UnknownProduct() => new(Guid.NewGuid());
}
