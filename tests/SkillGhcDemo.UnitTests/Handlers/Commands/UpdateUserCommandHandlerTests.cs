using FluentAssertions;
using SkillGhcDemo.Application.Common.Models;
using SkillGhcDemo.Application.Handlers.Commands.Users;
using SkillGhcDemo.Domain.Entities;
using SkillGhcDemo.UnitTests.Commands;
using SkillGhcDemo.UnitTests.Common;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace SkillGhcDemo.UnitTests.Handlers.Commands;

public class UpdateUserCommandHandlerTests
{
    [Fact]
    public async Task Succeeds_for_existing_user()
    {
        using var db = TestDbContextFactory.Create();
        var user = new User { FullName = "Old name", Email = "old@example.com" };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var handler = new UpdateUserCommandHandler(db, TestLogger.For<UpdateUserCommandHandler>());
        var result = await handler.Handle(UpdateUserCommandData.Valid(user.Id), CancellationToken.None);

        result.ShouldBeSuccess();
        (await db.Users.SingleAsync()).FullName.Should().Be("John Smith");
    }

    [Fact]
    public async Task Returns_notfound_for_unknown_id()
    {
        using var db = TestDbContextFactory.Create();
        var handler = new UpdateUserCommandHandler(db, TestLogger.For<UpdateUserCommandHandler>());

        var result = await handler.Handle(UpdateUserCommandData.Valid(Guid.NewGuid()), CancellationToken.None);

        result.ShouldBeFailureOfType(ResultErrorType.NotFound);
    }

    [Fact]
    public async Task Returns_conflict_when_email_taken_by_another_user()
    {
        using var db = TestDbContextFactory.Create();
        var first = new User { FullName = "First user", Email = "first@example.com" };
        var second = new User { FullName = "Second user", Email = "second@example.com" };
        db.Users.AddRange(first, second);
        await db.SaveChangesAsync();

        var handler = new UpdateUserCommandHandler(db, TestLogger.For<UpdateUserCommandHandler>());
        var result = await handler.Handle(UpdateUserCommandData.WithEmail(second.Id, "first@example.com"), CancellationToken.None);

        result.ShouldBeFailureOfType(ResultErrorType.Conflict);
    }
}
