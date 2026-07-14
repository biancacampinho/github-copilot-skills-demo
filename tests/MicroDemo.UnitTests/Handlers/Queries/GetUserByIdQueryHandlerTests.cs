using FluentAssertions;
using MicroDemo.Application.Common.Models;
using MicroDemo.Application.Handlers.Queries.Users;
using MicroDemo.Domain.Entities;
using MicroDemo.UnitTests.Common;
using MicroDemo.UnitTests.Queries;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MicroDemo.UnitTests.Handlers.Queries;

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
