using SkillGhcDemo.Application.Queries.Prices;

namespace SkillGhcDemo.UnitTests.Queries;

/// <summary>Test-data variants of <see cref="GetPricesQuery"/> for handler tests.</summary>
public static class GetPricesQueryData
{
    public static GetPricesQuery All() => new(ProductId: null, OnlyActive: false);

    public static GetPricesQuery OnlyActive() => new(ProductId: null, OnlyActive: true);

    public static GetPricesQuery ByProduct(Guid productId) => new(ProductId: productId, OnlyActive: false);
}
