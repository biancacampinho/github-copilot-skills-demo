using FluentAssertions;
using MicroDemo.Application.Common.Models;
using MicroDemo.Application.Handlers.Commands.Products;
using MicroDemo.Domain.Entities;
using MicroDemo.UnitTests.Commands;
using MicroDemo.UnitTests.Common;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MicroDemo.UnitTests.Handlers.Commands;

public class DeleteProductCommandHandlerTests
{
    [Fact]
    public async Task Succeeds_when_not_referenced()
    {
        using var db = TestDbContextFactory.Create();
        var category = new Category { Name = "Electronics" };
        var product = new Product { Name = "Headphone", Sku = "SKU-1", Category = category };
        db.Categories.Add(category);
        db.Products.Add(product);
        await db.SaveChangesAsync();

        var handler = new DeleteProductCommandHandler(db, TestLogger.For<DeleteProductCommandHandler>());
        var result = await handler.Handle(DeleteProductCommandData.ForId(product.Id), CancellationToken.None);

        result.ShouldBeSuccess();
        (await db.Products.CountAsync()).Should().Be(0);
    }

    [Fact]
    public async Task Returns_notfound_for_unknown_id()
    {
        using var db = TestDbContextFactory.Create();
        var handler = new DeleteProductCommandHandler(db, TestLogger.For<DeleteProductCommandHandler>());

        var result = await handler.Handle(DeleteProductCommandData.Unknown(), CancellationToken.None);

        result.ShouldBeFailureOfType(ResultErrorType.NotFound);
    }

    [Fact]
    public async Task Returns_conflict_when_order_items_exist()
    {
        using var db = TestDbContextFactory.Create();
        var category = new Category { Name = "Electronics" };
        var product = new Product { Name = "Headphone", Sku = "SKU-1", Category = category };
        var user = new User { FullName = "John Doe", Email = "john@example.com" };
        var order = new Order { User = user };
        var orderItem = new OrderItem { Order = order, Product = product, Quantity = 1, UnitPrice = 10m, LineTotal = 10m };
        db.Categories.Add(category);
        db.Products.Add(product);
        db.Users.Add(user);
        db.Orders.Add(order);
        db.OrderItems.Add(orderItem);
        await db.SaveChangesAsync();

        var handler = new DeleteProductCommandHandler(db, TestLogger.For<DeleteProductCommandHandler>());
        var result = await handler.Handle(DeleteProductCommandData.ForId(product.Id), CancellationToken.None);

        result.ShouldBeFailureOfType(ResultErrorType.Conflict);
    }
}
