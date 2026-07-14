using SkillGhcDemo.Application.Commands.Users;

namespace SkillGhcDemo.UnitTests.Commands;

/// <summary>Test-data variants of <see cref="CreateUserCommand"/> for handler and validator tests.</summary>
public static class CreateUserCommandData
{
    public static CreateUserCommand Valid() => new()
    {
        FullName = "John Smith",
        Email = "john.smith@example.com",
        PhoneNumber = "+39 333 1234567"
    };

    public static CreateUserCommand WithEmail(string email) => Valid() with { Email = email };

    public static CreateUserCommand WithInvalidEmail() => Valid() with { Email = "not-an-email" };

    public static CreateUserCommand WithEmptyName() => Valid() with { FullName = "" };
}
