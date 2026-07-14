using FluentAssertions;
using SkillGhcDemo.Application.Common.Models;
using SkillGhcDemo.Application.Handlers.Commands.Products;
using SkillGhcDemo.Domain.Entities;
using SkillGhcDemo.UnitTests.Commands;
using SkillGhcDemo.UnitTests.Common;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace SkillGhcDemo.UnitTests.Handlers.Commands;

public class CreateProductCommandHandlerTests
{
    [Fact]
    public async Task Succeeds_when_category_exists()
    {
        using var db = TestDbContextFactory.Create();
        var category = new Category { Name = "Electronics" };
        db.Categories.Add(category);
        await db.SaveChangesAsync();

        var handler = new CreateProductCommandHandler(db, TestLogger.For<CreateProductCommandHandler>());
        var result = await handler.Handle(CreateProductCommandData.Valid(category.Id), CancellationToken.None);

        result.ShouldBeSuccess();
        (await db.Products.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task Returns_notfound_when_category_missing()
    {
        using var db = TestDbContextFactory.Create();
        var handler = new CreateProductCommandHandler(db, TestLogger.For<CreateProductCommandHandler>());

        var result = await handler.Handle(CreateProductCommandData.Valid(Guid.NewGuid()), CancellationToken.None);

        result.ShouldBeFailureOfType(ResultErrorType.NotFound);
    }

    [Fact]
    public async Task Returns_conflict_for_duplicate_sku()
    {
        using var db = TestDbContextFactory.Create();
        var category = new Category { Name = "Electronics" };
        db.Categories.Add(category);
        db.Products.Add(new Product { Name = "Existing", Sku = "SKU-DUP", Category = category });
        await db.SaveChangesAsync();

        var handler = new CreateProductCommandHandler(db, TestLogger.For<CreateProductCommandHandler>());
        var result = await handler.Handle(CreateProductCommandData.Valid(category.Id) with { Sku = "SKU-DUP" }, CancellationToken.None);

        result.ShouldBeFailureOfType(ResultErrorType.Conflict);
    }
}
