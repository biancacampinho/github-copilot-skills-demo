using FluentValidation.TestHelper;
using SkillGhcDemo.Application.Commands.Categories;
using SkillGhcDemo.UnitTests.Commands;
using Xunit;

namespace SkillGhcDemo.UnitTests.Validators;

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
