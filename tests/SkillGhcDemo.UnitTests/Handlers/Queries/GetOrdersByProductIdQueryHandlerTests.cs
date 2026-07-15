using FluentAssertions;
using SkillGhcDemo.Application.Common.Models;
using SkillGhcDemo.Application.Handlers.Queries.Orders;
using SkillGhcDemo.Domain.Entities;
using SkillGhcDemo.UnitTests.Common;
using SkillGhcDemo.UnitTests.Queries;
using Xunit;

namespace SkillGhcDemo.UnitTests.Handlers.Queries;

public class GetOrdersByProductIdQueryHandlerTests
{
    [Fact]
    public async Task Returns_order_lines_with_user_and_product_for_existing_product()
    {
        using var db = TestDbContextFactory.Create();

        var category = new Category { Name = "Electronics" };
        var user = new User { FullName = "Ada Lovelace", Email = "ada@example.com" };
        var product = new Product { Name = "Keyboard", Sku = "KB-001", Category = category };
        db.Categories.Add(category);
        db.Users.Add(user);
        db.Products.Add(product);
        await db.SaveChangesAsync();

        var order = new Order
        {
            UserId = user.Id,
            Currency = "EUR",
            TotalAmount = 40m,
            Items = new List<OrderItem>
            {
                new()
                {
                    ProductId = product.Id,
                    Quantity = 2,
                    UnitPrice = 20m,
                    Currency = "EUR",
                    LineTotal = 40m
                }
            }
        };
        db.Orders.Add(order);
        await db.SaveChangesAsync();

        var handler = new GetOrdersByProductIdQueryHandler(db);
        var result = await handler.Handle(GetOrdersByProductIdQueryData.ForProduct(product.Id), CancellationToken.None);

        result.ShouldBeSuccess();
        var line = result.Data.Should().ContainSingle().Which;
        line.OrderId.Should().Be(order.Id);
        line.Quantity.Should().Be(2);
        line.User.Id.Should().Be(user.Id);
        line.Product.Id.Should().Be(product.Id);
    }

    [Fact]
    public async Task Returns_empty_list_when_product_has_no_orders()
    {
        using var db = TestDbContextFactory.Create();
        var category = new Category { Name = "Electronics" };
        var product = new Product { Name = "Mouse", Sku = "MOUSE-001", Category = category };
        db.Categories.Add(category);
        db.Products.Add(product);
        await db.SaveChangesAsync();

        var handler = new GetOrdersByProductIdQueryHandler(db);
        var result = await handler.Handle(GetOrdersByProductIdQueryData.ForProduct(product.Id), CancellationToken.None);

        result.ShouldBeSuccess();
        result.Data.Should().BeEmpty();
    }

    [Fact]
    public async Task Returns_notfound_for_unknown_product()
    {
        using var db = TestDbContextFactory.Create();
        var handler = new GetOrdersByProductIdQueryHandler(db);

        var result = await handler.Handle(GetOrdersByProductIdQueryData.UnknownProduct(), CancellationToken.None);

        result.ShouldBeFailureOfType(ResultErrorType.NotFound);
    }
}
