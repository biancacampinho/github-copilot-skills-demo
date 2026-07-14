using FluentAssertions;
using MicroDemo.Application.Common.Models;
using MicroDemo.Application.Handlers.Commands.Users;
using MicroDemo.Domain.Entities;
using MicroDemo.UnitTests.Commands;
using MicroDemo.UnitTests.Common;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MicroDemo.UnitTests.Handlers.Commands;

public class DeleteUserCommandHandlerTests
{
    [Fact]
    public async Task Succeeds_when_no_orders()
    {
        using var db = TestDbContextFactory.Create();
        var user = new User { FullName = "No orders", Email = "u@example.com" };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var handler = new DeleteUserCommandHandler(db, TestLogger.For<DeleteUserCommandHandler>());
        var result = await handler.Handle(DeleteUserCommandData.ForId(user.Id), CancellationToken.None);

        result.ShouldBeSuccess();
        (await db.Users.CountAsync()).Should().Be(0);
    }

    [Fact]
    public async Task Returns_notfound_for_unknown_id()
    {
        using var db = TestDbContextFactory.Create();
        var handler = new DeleteUserCommandHandler(db, TestLogger.For<DeleteUserCommandHandler>());

        var result = await handler.Handle(DeleteUserCommandData.Unknown(), CancellationToken.None);

        result.ShouldBeFailureOfType(ResultErrorType.NotFound);
    }

    [Fact]
    public async Task Returns_conflict_when_orders_exist()
    {
        using var db = TestDbContextFactory.Create();
        var user = new User { FullName = "With order", Email = "u@example.com" };
        db.Users.Add(user);
        db.Orders.Add(new Order { User = user, TotalAmount = 10m, Currency = "EUR" });
        await db.SaveChangesAsync();

        var handler = new DeleteUserCommandHandler(db, TestLogger.For<DeleteUserCommandHandler>());
        var result = await handler.Handle(DeleteUserCommandData.ForId(user.Id), CancellationToken.None);

        result.ShouldBeFailureOfType(ResultErrorType.Conflict);
    }
}
