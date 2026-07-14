using FluentValidation.TestHelper;
using SkillGhcDemo.Application.Commands.Users;
using SkillGhcDemo.UnitTests.Commands;
using Xunit;

namespace SkillGhcDemo.UnitTests.Validators;

public class CreateUserCommandValidatorTests
{
    private readonly CreateUserCommandValidator _validator = new();

    [Fact]
    public void Valid_request_passes()
    {
        var result = _validator.TestValidate(CreateUserCommandData.Valid());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Invalid_email_fails()
    {
        var result = _validator.TestValidate(CreateUserCommandData.WithInvalidEmail());
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Empty_name_fails()
    {
        var result = _validator.TestValidate(CreateUserCommandData.WithEmptyName());
        result.ShouldHaveValidationErrorFor(x => x.FullName);
    }
}
