using FluentAssertions;
using SkillGhcDemo.Application.Common.Models;
using SkillGhcDemo.Application.Handlers.Commands.Categories;
using SkillGhcDemo.Domain.Entities;
using SkillGhcDemo.UnitTests.Commands;
using SkillGhcDemo.UnitTests.Common;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace SkillGhcDemo.UnitTests.Handlers.Commands;

public class UpdateCategoryCommandHandlerTests
{
    [Fact]
    public async Task Succeeds_for_existing_category()
    {
        using var db = TestDbContextFactory.Create();
        var category = new Category { Name = "Old name" };
        db.Categories.Add(category);
        await db.SaveChangesAsync();

        var handler = new UpdateCategoryCommandHandler(db, TestLogger.For<UpdateCategoryCommandHandler>());
        var result = await handler.Handle(UpdateCategoryCommandData.WithName(category.Id, "New name"), CancellationToken.None);

        result.ShouldBeSuccess();
        (await db.Categories.SingleAsync()).Name.Should().Be("New name");
    }

    [Fact]
    public async Task Returns_notfound_for_unknown_id()
    {
        using var db = TestDbContextFactory.Create();
        var handler = new UpdateCategoryCommandHandler(db, TestLogger.For<UpdateCategoryCommandHandler>());

        var result = await handler.Handle(UpdateCategoryCommandData.Valid(Guid.NewGuid()), CancellationToken.None);

        result.ShouldBeFailureOfType(ResultErrorType.NotFound);
    }

    [Fact]
    public async Task Returns_conflict_when_name_taken_by_another_category()
    {
        using var db = TestDbContextFactory.Create();
        var electronics = new Category { Name = "Electronics" };
        var books = new Category { Name = "Books" };
        db.Categories.AddRange(electronics, books);
        await db.SaveChangesAsync();

        var handler = new UpdateCategoryCommandHandler(db, TestLogger.For<UpdateCategoryCommandHandler>());
        var result = await handler.Handle(UpdateCategoryCommandData.WithName(books.Id, "Electronics"), CancellationToken.None);

        result.ShouldBeFailureOfType(ResultErrorType.Conflict);
    }
}
