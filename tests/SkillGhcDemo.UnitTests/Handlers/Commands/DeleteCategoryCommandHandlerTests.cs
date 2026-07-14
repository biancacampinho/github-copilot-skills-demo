using FluentAssertions;
using SkillGhcDemo.Application.Common.Models;
using SkillGhcDemo.Application.Handlers.Commands.Categories;
using SkillGhcDemo.Domain.Entities;
using SkillGhcDemo.UnitTests.Commands;
using SkillGhcDemo.UnitTests.Common;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace SkillGhcDemo.UnitTests.Handlers.Commands;

public class DeleteCategoryCommandHandlerTests
{
    [Fact]
    public async Task Succeeds_when_no_products_are_associated()
    {
        using var db = TestDbContextFactory.Create();
        var category = new Category { Name = "Electronics" };
        db.Categories.Add(category);
        await db.SaveChangesAsync();

        var handler = new DeleteCategoryCommandHandler(db, TestLogger.For<DeleteCategoryCommandHandler>());
        var result = await handler.Handle(DeleteCategoryCommandData.ForId(category.Id), CancellationToken.None);

        result.ShouldBeSuccess();
        (await db.Categories.CountAsync()).Should().Be(0);
    }

    [Fact]
    public async Task Returns_notfound_for_unknown_id()
    {
        using var db = TestDbContextFactory.Create();
        var handler = new DeleteCategoryCommandHandler(db, TestLogger.For<DeleteCategoryCommandHandler>());

        var result = await handler.Handle(DeleteCategoryCommandData.Unknown(), CancellationToken.None);

        result.ShouldBeFailureOfType(ResultErrorType.NotFound);
    }

    [Fact]
    public async Task Returns_conflict_when_products_exist()
    {
        using var db = TestDbContextFactory.Create();
        var category = new Category { Name = "With products" };
        db.Categories.Add(category);
        db.Products.Add(new Product { Name = "P", Sku = "SKU-1", Category = category });
        await db.SaveChangesAsync();

        var handler = new DeleteCategoryCommandHandler(db, TestLogger.For<DeleteCategoryCommandHandler>());
        var result = await handler.Handle(DeleteCategoryCommandData.ForId(category.Id), CancellationToken.None);

        result.ShouldBeFailureOfType(ResultErrorType.Conflict);
    }
}
