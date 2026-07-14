using FluentValidation.TestHelper;
using MicroDemo.Application.Commands.Products;
using MicroDemo.UnitTests.Commands;
using Xunit;

namespace MicroDemo.UnitTests.Validators;

public class UpdateProductCommandValidatorTests
{
    private readonly UpdateProductCommandValidator _validator = new();

    [Fact]
    public void Valid_request_passes()
    {
        var result = _validator.TestValidate(UpdateProductCommandData.Valid(Guid.NewGuid()));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Empty_id_fails()
    {
        var result = _validator.TestValidate(UpdateProductCommandData.WithEmptyId());
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Empty_name_fails()
    {
        var result = _validator.TestValidate(UpdateProductCommandData.WithEmptyName(Guid.NewGuid()));
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }
}
