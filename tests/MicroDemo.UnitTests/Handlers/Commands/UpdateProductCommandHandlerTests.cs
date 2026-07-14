using FluentAssertions;
using MicroDemo.Application.Common.Models;
using MicroDemo.Application.Handlers.Commands.Products;
using MicroDemo.Domain.Entities;
using MicroDemo.UnitTests.Commands;
using MicroDemo.UnitTests.Common;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MicroDemo.UnitTests.Handlers.Commands;

public class UpdateProductCommandHandlerTests
{
    [Fact]
    public async Task Succeeds_for_existing_product()
    {
        using var db = TestDbContextFactory.Create();
        var category = new Category { Name = "Electronics" };
        var product = new Product { Name = "Old name", Sku = "SKU-OLD", Category = category };
        db.Categories.Add(category);
        db.Products.Add(product);
        await db.SaveChangesAsync();

        var handler = new UpdateProductCommandHandler(db, TestLogger.For<UpdateProductCommandHandler>());
        var command = UpdateProductCommandData.Valid(product.Id, category.Id) with { Name = "New name" };
        var result = await handler.Handle(command, CancellationToken.None);

        result.ShouldBeSuccess();
        (await db.Products.SingleAsync()).Name.Should().Be("New name");
    }

    [Fact]
    public async Task Returns_notfound_for_unknown_id()
    {
        using var db = TestDbContextFactory.Create();
        var handler = new UpdateProductCommandHandler(db, TestLogger.For<UpdateProductCommandHandler>());

        var result = await handler.Handle(UpdateProductCommandData.Valid(Guid.NewGuid()), CancellationToken.None);

        result.ShouldBeFailureOfType(ResultErrorType.NotFound);
    }

    [Fact]
    public async Task Returns_notfound_when_category_missing()
    {
        using var db = TestDbContextFactory.Create();
        var category = new Category { Name = "Electronics" };
        var product = new Product { Name = "Product", Sku = "SKU-1", Category = category };
        db.Categories.Add(category);
        db.Products.Add(product);
        await db.SaveChangesAsync();

        var handler = new UpdateProductCommandHandler(db, TestLogger.For<UpdateProductCommandHandler>());
        var result = await handler.Handle(UpdateProductCommandData.Valid(product.Id, Guid.NewGuid()), CancellationToken.None);

        result.ShouldBeFailureOfType(ResultErrorType.NotFound);
    }

    [Fact]
    public async Task Returns_conflict_when_sku_taken_by_another_product()
    {
        using var db = TestDbContextFactory.Create();
        var category = new Category { Name = "Electronics" };
        var first = new Product { Name = "First", Sku = "SKU-A", Category = category };
        var second = new Product { Name = "Second", Sku = "SKU-B", Category = category };
        db.Categories.Add(category);
        db.Products.AddRange(first, second);
        await db.SaveChangesAsync();

        var handler = new UpdateProductCommandHandler(db, TestLogger.For<UpdateProductCommandHandler>());
        var command = UpdateProductCommandData.Valid(second.Id, category.Id) with { Sku = "SKU-A" };
        var result = await handler.Handle(command, CancellationToken.None);

        result.ShouldBeFailureOfType(ResultErrorType.Conflict);
    }
}
