using SkillGhcDemo.Application.Commands.Prices;

namespace SkillGhcDemo.UnitTests.Commands;

/// <summary>Test-data variants of <see cref="UpdatePriceCommand"/> for handler and validator tests.</summary>
public static class UpdatePriceCommandData
{
    public static readonly DateTime ValidFrom = new(2026, 01, 01, 0, 0, 0, DateTimeKind.Utc);

    public static UpdatePriceCommand Valid(Guid id) => new()
    {
        Id = id,
        Amount = 29.90m,
        Currency = "EUR",
        ValidFromUtc = ValidFrom,
        IsActive = true
    };

    public static UpdatePriceCommand WithEmptyId() => Valid(Guid.Empty);

    public static UpdatePriceCommand WithNegativeAmount(Guid id) => Valid(id) with { Amount = -1m };

    public static UpdatePriceCommand WithInvalidCurrency(Guid id) => Valid(id) with { Currency = "EURO" };
}
