using FluentValidation.TestHelper;
using MicroDemo.Application.Commands.Prices;
using MicroDemo.UnitTests.Commands;
using Xunit;

namespace MicroDemo.UnitTests.Validators;

public class UpdatePriceCommandValidatorTests
{
    private readonly UpdatePriceCommandValidator _validator = new();

    [Fact]
    public void Valid_request_passes()
    {
        var result = _validator.TestValidate(UpdatePriceCommandData.Valid(Guid.NewGuid()));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Empty_id_fails()
    {
        var result = _validator.TestValidate(UpdatePriceCommandData.WithEmptyId());
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Negative_amount_fails()
    {
        var result = _validator.TestValidate(UpdatePriceCommandData.WithNegativeAmount(Guid.NewGuid()));
        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }

    [Fact]
    public void Invalid_currency_length_fails()
    {
        var result = _validator.TestValidate(UpdatePriceCommandData.WithInvalidCurrency(Guid.NewGuid()));
        result.ShouldHaveValidationErrorFor(x => x.Currency);
    }
}
