using SkillGhcDemo.Application.Commands.Prices;

namespace SkillGhcDemo.UnitTests.Commands;

/// <summary>Test-data variants of <see cref="DeletePriceCommand"/> for handler tests.</summary>
public static class DeletePriceCommandData
{
    public static DeletePriceCommand ForId(Guid id) => new(id);

    public static DeletePriceCommand Unknown() => new(Guid.NewGuid());
}
