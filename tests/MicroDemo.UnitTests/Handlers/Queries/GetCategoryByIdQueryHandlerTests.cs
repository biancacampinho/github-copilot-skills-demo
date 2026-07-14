using FluentAssertions;
using MicroDemo.Application.Common.Models;
using MicroDemo.Application.Handlers.Queries.Categories;
using MicroDemo.Domain.Entities;
using MicroDemo.UnitTests.Common;
using MicroDemo.UnitTests.Queries;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MicroDemo.UnitTests.Handlers.Queries;

public class GetCategoryByIdQueryHandlerTests
{
    [Fact]
    public async Task Returns_category_for_existing_id()
    {
        using var db = TestDbContextFactory.Create();
        var category = new Category { Name = "Electronics" };
        db.Categories.Add(category);
        await db.SaveChangesAsync();

        var handler = new GetCategoryByIdQueryHandler(db);
        var result = await handler.Handle(GetCategoryByIdQueryData.ForId(category.Id), CancellationToken.None);

        result.ShouldBeSuccess();
        result.Data!.Name.Should().Be("Electronics");
    }

    [Fact]
    public async Task Returns_notfound_for_unknown_id()
    {
        using var db = TestDbContextFactory.Create();
        var handler = new GetCategoryByIdQueryHandler(db);

        var result = await handler.Handle(GetCategoryByIdQueryData.Unknown(), CancellationToken.None);

        result.ShouldBeFailureOfType(ResultErrorType.NotFound);
    }
}
