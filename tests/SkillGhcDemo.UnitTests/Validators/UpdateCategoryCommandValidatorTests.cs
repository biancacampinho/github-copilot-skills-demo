using FluentValidation.TestHelper;
using SkillGhcDemo.Application.Commands.Categories;
using SkillGhcDemo.UnitTests.Commands;
using Xunit;

namespace SkillGhcDemo.UnitTests.Validators;

public class UpdateCategoryCommandValidatorTests
{
    private readonly UpdateCategoryCommandValidator _validator = new();

    [Fact]
    public void Valid_request_passes()
    {
        var result = _validator.TestValidate(UpdateCategoryCommandData.Valid(Guid.NewGuid()));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Empty_id_fails()
    {
        var result = _validator.TestValidate(UpdateCategoryCommandData.WithEmptyId());
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Empty_name_fails()
    {
        var result = _validator.TestValidate(UpdateCategoryCommandData.WithEmptyName(Guid.NewGuid()));
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }
}
