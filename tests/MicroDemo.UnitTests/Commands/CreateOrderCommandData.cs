using MicroDemo.Application.Commands.Orders;

namespace MicroDemo.UnitTests.Commands;

/// <summary>Test-data variants of <see cref="CreateOrderCommand"/> for handler and validator tests.</summary>
public static class CreateOrderCommandData
{
    public static CreateOrderCommand Valid(Guid? userId = null, Guid? productId = null) => new()
    {
        UserId = userId ?? Guid.NewGuid(),
        Items = new List<CreateOrderItem>
        {
            new() { ProductId = productId ?? Guid.NewGuid(), Quantity = 2 }
        }
    };

    public static CreateOrderCommand WithNoItems() => Valid() with { Items = new List<CreateOrderItem>() };

    public static CreateOrderCommand WithZeroQuantity() => Valid() with
    {
        Items = new List<CreateOrderItem> { new() { ProductId = Guid.NewGuid(), Quantity = 0 } }
    };

    public static CreateOrderCommand WithEmptyUser() => Valid() with { UserId = Guid.Empty };
}
