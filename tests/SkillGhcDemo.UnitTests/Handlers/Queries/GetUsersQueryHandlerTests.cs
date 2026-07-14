using FluentAssertions;
using SkillGhcDemo.Application.Handlers.Queries.Users;
using SkillGhcDemo.Domain.Entities;
using SkillGhcDemo.UnitTests.Common;
using SkillGhcDemo.UnitTests.Queries;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace SkillGhcDemo.UnitTests.Handlers.Queries;

public class GetUsersQueryHandlerTests
{
    [Fact]
    public async Task Returns_all_users()
    {
        using var db = TestDbContextFactory.Create();
        db.Users.AddRange(
            new User { FullName = "John Smith", Email = "john@example.com" },
            new User { FullName = "Jane Doe", Email = "jane@example.com" });
        await db.SaveChangesAsync();

        var handler = new GetUsersQueryHandler(db);
        var result = await handler.Handle(GetUsersQueryData.All(), CancellationToken.None);

        result.ShouldBeSuccess();
        result.Data!.Should().HaveCount(2);
    }

    [Fact]
    public async Task OnlyActive_filters_inactive_users()
    {
        using var db = TestDbContextFactory.Create();
        db.Users.Add(new User { FullName = "Active", Email = "active@example.com", IsActive = true });
        db.Users.Add(new User { FullName = "Inactive", Email = "inactive@example.com", IsActive = false });
        await db.SaveChangesAsync();

        var handler = new GetUsersQueryHandler(db);
        var result = await handler.Handle(GetUsersQueryData.OnlyActive(), CancellationToken.None);

        result.ShouldBeSuccess();
        result.Data!.Should().ContainSingle().Which.FullName.Should().Be("Active");
    }
}
