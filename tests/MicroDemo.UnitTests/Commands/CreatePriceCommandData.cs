using MicroDemo.Application.Commands.Prices;

namespace MicroDemo.UnitTests.Commands;

/// <summary>Test-data variants of <see cref="CreatePriceCommand"/> for handler and validator tests.</summary>
public static class CreatePriceCommandData
{
    public static readonly DateTime ValidFrom = new(2026, 01, 01, 0, 0, 0, DateTimeKind.Utc);

    public static CreatePriceCommand Valid(Guid? productId = null) => new()
    {
        ProductId = productId ?? Guid.NewGuid(),
        Amount = 29.90m,
        Currency = "EUR",
        ValidFromUtc = ValidFrom,
        IsActive = true
    };

    public static CreatePriceCommand WithNegativeAmount(Guid? productId = null) => Valid(productId) with { Amount = -1m };

    public static CreatePriceCommand WithInvalidCurrency(Guid? productId = null) => Valid(productId) with { Currency = "EURO" };

    public static CreatePriceCommand WithEmptyProduct() => Valid() with { ProductId = Guid.Empty };
}
