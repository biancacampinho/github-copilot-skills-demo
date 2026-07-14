using FluentAssertions;
using MicroDemo.Application.Common.Models;
using MicroDemo.Application.Handlers.Commands.Users;
using MicroDemo.Domain.Entities;
using MicroDemo.UnitTests.Commands;
using MicroDemo.UnitTests.Common;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MicroDemo.UnitTests.Handlers.Commands;

public class CreateUserCommandHandlerTests
{
    [Fact]
    public async Task Succeeds_for_valid_request()
    {
        using var db = TestDbContextFactory.Create();
        var handler = new CreateUserCommandHandler(db, TestLogger.For<CreateUserCommandHandler>());

        var result = await handler.Handle(CreateUserCommandData.Valid(), CancellationToken.None);

        result.ShouldBeSuccess();
        (await db.Users.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task Returns_conflict_for_duplicate_email()
    {
        using var db = TestDbContextFactory.Create();
        db.Users.Add(new User { FullName = "Existing user", Email = "john.smith@example.com" });
        await db.SaveChangesAsync();

        var handler = new CreateUserCommandHandler(db, TestLogger.For<CreateUserCommandHandler>());
        var result = await handler.Handle(CreateUserCommandData.WithEmail("john.smith@example.com"), CancellationToken.None);

        result.ShouldBeFailureOfType(ResultErrorType.Conflict);
    }
}
