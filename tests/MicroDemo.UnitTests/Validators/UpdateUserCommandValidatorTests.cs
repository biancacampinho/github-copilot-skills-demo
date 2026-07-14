using FluentValidation.TestHelper;
using MicroDemo.Application.Commands.Users;
using MicroDemo.UnitTests.Commands;
using Xunit;

namespace MicroDemo.UnitTests.Validators;

public class UpdateUserCommandValidatorTests
{
    private readonly UpdateUserCommandValidator _validator = new();

    [Fact]
    public void Valid_request_passes()
    {
        var result = _validator.TestValidate(UpdateUserCommandData.Valid(Guid.NewGuid()));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Empty_id_fails()
    {
        var result = _validator.TestValidate(UpdateUserCommandData.WithEmptyId());
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Empty_name_fails()
    {
        var result = _validator.TestValidate(UpdateUserCommandData.WithEmptyName(Guid.NewGuid()));
        result.ShouldHaveValidationErrorFor(x => x.FullName);
    }
}
