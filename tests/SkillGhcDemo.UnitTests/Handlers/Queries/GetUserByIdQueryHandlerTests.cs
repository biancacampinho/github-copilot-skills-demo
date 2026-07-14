using FluentAssertions;
using SkillGhcDemo.Application.Common.Models;
using SkillGhcDemo.Application.Handlers.Queries.Users;
using SkillGhcDemo.Domain.Entities;
using SkillGhcDemo.UnitTests.Common;
using SkillGhcDemo.UnitTests.Queries;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace SkillGhcDemo.UnitTests.Handlers.Queries;

public class GetUserByIdQueryHandlerTests
{
    [Fact]
    public async Task Returns_user_for_existing_id()
    {
        using var db = TestDbContextFactory.Create();
        var user = new User { FullName = "John Smith", Email = "john@example.com" };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var handler = new GetUserByIdQueryHandler(db);
        var result = await handler.Handle(GetUserByIdQueryData.ForId(user.Id), CancellationToken.None);

        result.ShouldBeSuccess();
        result.Data!.FullName.Should().Be("John Smith");
    }

    [Fact]
    public async Task Returns_notfound_for_unknown_id()
    {
        using var db = TestDbContextFactory.Create();
        var handler = new GetUserByIdQueryHandler(db);

        var result = await handler.Handle(GetUserByIdQueryData.Unknown(), CancellationToken.None);

        result.ShouldBeFailureOfType(ResultErrorType.NotFound);
    }
}
