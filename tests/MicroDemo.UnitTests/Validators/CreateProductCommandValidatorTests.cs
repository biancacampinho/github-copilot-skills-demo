using FluentValidation.TestHelper;
using MicroDemo.Application.Commands.Products;
using MicroDemo.UnitTests.Commands;
using Xunit;

namespace MicroDemo.UnitTests.Validators;

public class CreateProductCommandValidatorTests
{
    private readonly CreateProductCommandValidator _validator = new();

    [Fact]
    public void Valid_request_passes()
    {
        var result = _validator.TestValidate(CreateProductCommandData.Valid());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Empty_name_fails()
    {
        var result = _validator.TestValidate(CreateProductCommandData.WithEmptyName());
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Empty_sku_fails()
    {
        var result = _validator.TestValidate(CreateProductCommandData.WithEmptySku());
        result.ShouldHaveValidationErrorFor(x => x.Sku);
    }

    [Fact]
    public void Empty_category_fails()
    {
        var result = _validator.TestValidate(CreateProductCommandData.WithEmptyCategory());
        result.ShouldHaveValidationErrorFor(x => x.CategoryId);
    }
}
