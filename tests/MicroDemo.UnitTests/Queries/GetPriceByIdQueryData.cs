using MicroDemo.Application.Queries.Prices;

namespace MicroDemo.UnitTests.Queries;

/// <summary>Test-data variants of <see cref="GetPriceByIdQuery"/> for handler tests.</summary>
public static class GetPriceByIdQueryData
{
    public static GetPriceByIdQuery ForId(Guid id) => new(id);

    public static GetPriceByIdQuery Unknown() => new(Guid.NewGuid());
}
