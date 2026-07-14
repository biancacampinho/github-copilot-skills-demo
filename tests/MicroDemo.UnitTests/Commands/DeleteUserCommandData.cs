using MicroDemo.Application.Commands.Users;

namespace MicroDemo.UnitTests.Commands;

/// <summary>Test-data variants of <see cref="DeleteUserCommand"/> for handler tests.</summary>
public static class DeleteUserCommandData
{
    public static DeleteUserCommand ForId(Guid id) => new(id);

    public static DeleteUserCommand Unknown() => new(Guid.NewGuid());
}
