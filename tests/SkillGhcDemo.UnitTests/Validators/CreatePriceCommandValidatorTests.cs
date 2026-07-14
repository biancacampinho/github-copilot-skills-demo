using FluentValidation.TestHelper;
using SkillGhcDemo.Application.Commands.Prices;
using SkillGhcDemo.UnitTests.Commands;
using Xunit;

namespace SkillGhcDemo.UnitTests.Validators;

public class CreatePriceCommandValidatorTests
{
    private readonly CreatePriceCommandValidator _validator = new();

    [Fact]
    public void Valid_request_passes()
    {
        var result = _validator.TestValidate(CreatePriceCommandData.Valid());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Negative_amount_fails()
    {
        var result = _validator.TestValidate(CreatePriceCommandData.WithNegativeAmount());
        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }

    [Fact]
    public void Invalid_currency_length_fails()
    {
        var result = _validator.TestValidate(CreatePriceCommandData.WithInvalidCurrency());
        result.ShouldHaveValidationErrorFor(x => x.Currency);
    }

    [Fact]
    public void Empty_product_fails()
    {
        var result = _validator.TestValidate(CreatePriceCommandData.WithEmptyProduct());
        result.ShouldHaveValidationErrorFor(x => x.ProductId);
    }
}
