using MicroDemo.Application.Commands.Products;

namespace MicroDemo.UnitTests.Commands;

/// <summary>Test-data variants of <see cref="DeleteProductCommand"/> for handler tests.</summary>
public static class DeleteProductCommandData
{
    public static DeleteProductCommand ForId(Guid id) => new(id);

    public static DeleteProductCommand Unknown() => new(Guid.NewGuid());
}
