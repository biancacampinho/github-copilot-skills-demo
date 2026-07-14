using FluentAssertions;
using MicroDemo.Application.Handlers.Queries.Categories;
using MicroDemo.Domain.Entities;
using MicroDemo.UnitTests.Common;
using MicroDemo.UnitTests.Queries;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MicroDemo.UnitTests.Handlers.Queries;

public class GetCategoriesQueryHandlerTests
{
    [Fact]
    public async Task Returns_all_categories()
    {
        using var db = TestDbContextFactory.Create();
        db.Categories.AddRange(
            new Category { Name = "Electronics" },
            new Category { Name = "Books" });
        await db.SaveChangesAsync();

        var handler = new GetCategoriesQueryHandler(db);
        var result = await handler.Handle(GetCategoriesQueryData.All(), CancellationToken.None);

        result.ShouldBeSuccess();
        result.Data!.Should().HaveCount(2);
    }

    [Fact]
    public async Task OnlyActive_filters_inactive_categories()
    {
        using var db = TestDbContextFactory.Create();
        db.Categories.Add(new Category { Name = "Active", IsActive = true });
        db.Categories.Add(new Category { Name = "Inactive", IsActive = false });
        await db.SaveChangesAsync();

        var handler = new GetCategoriesQueryHandler(db);
        var result = await handler.Handle(GetCategoriesQueryData.OnlyActive(), CancellationToken.None);

        result.ShouldBeSuccess();
        result.Data!.Should().ContainSingle().Which.Name.Should().Be("Active");
    }
}
