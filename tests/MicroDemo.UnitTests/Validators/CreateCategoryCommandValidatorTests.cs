using FluentValidation.TestHelper;
using MicroDemo.Application.Commands.Categories;
using MicroDemo.UnitTests.Commands;
using Xunit;

namespace MicroDemo.UnitTests.Validators;

public class CreateCategoryCommandValidatorTests
{
    private readonly CreateCategoryCommandValidator _validator = new();

    [Fact]
    public void Valid_request_passes()
    {
        var result = _validator.TestValidate(CreateCategoryCommandData.Valid());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Empty_name_fails()
    {
        var result = _validator.TestValidate(CreateCategoryCommandData.WithEmptyName());
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Too_long_name_fails()
    {
        var result = _validator.TestValidate(CreateCategoryCommandData.WithTooLongName());
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }
}
