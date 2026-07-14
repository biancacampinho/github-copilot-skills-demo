using FluentValidation.TestHelper;
using MicroDemo.Application.Commands.Orders;
using MicroDemo.UnitTests.Commands;
using Xunit;

namespace MicroDemo.UnitTests.Validators;

public class CreateOrderCommandValidatorTests
{
    private readonly CreateOrderCommandValidator _validator = new();

    [Fact]
    public void Valid_request_passes()
    {
        var result = _validator.TestValidate(CreateOrderCommandData.Valid());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void No_items_fails()
    {
        var result = _validator.TestValidate(CreateOrderCommandData.WithNoItems());
        result.ShouldHaveValidationErrorFor(x => x.Items);
    }

    [Fact]
    public void Zero_quantity_fails()
    {
        var result = _validator.TestValidate(CreateOrderCommandData.WithZeroQuantity());
        result.ShouldHaveValidationErrorFor("Items[0].Quantity");
    }

    [Fact]
    public void Empty_user_fails()
    {
        var result = _validator.TestValidate(CreateOrderCommandData.WithEmptyUser());
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }
}
