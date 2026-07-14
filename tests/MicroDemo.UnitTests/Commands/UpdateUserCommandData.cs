using MicroDemo.Application.Commands.Users;

namespace MicroDemo.UnitTests.Commands;

/// <summary>Test-data variants of <see cref="UpdateUserCommand"/> for handler and validator tests.</summary>
public static class UpdateUserCommandData
{
    public static UpdateUserCommand Valid(Guid id) => new()
    {
        Id = id,
        FullName = "John Smith",
        Email = "john.smith@example.com",
        PhoneNumber = "+39 333 1234567",
        IsActive = true
    };

    public static UpdateUserCommand WithEmail(Guid id, string email) => Valid(id) with { Email = email };

    public static UpdateUserCommand WithEmptyName(Guid id) => Valid(id) with { FullName = "" };

    public static UpdateUserCommand WithEmptyId() => Valid(Guid.Empty);
}
