using FluentAssertions;
using MicroDemo.Application.Common.Models;
using MicroDemo.Application.Handlers.Queries.Orders;
using MicroDemo.Domain.Entities;
using MicroDemo.UnitTests.Common;
using MicroDemo.UnitTests.Queries;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MicroDemo.UnitTests.Handlers.Queries;

public class GetOrderByIdQueryHandlerTests
{
    [Fact]
    public async Task Returns_order_for_existing_id()
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

        var handler = new GetOrderByIdQueryHandler(db);
        var result = await handler.Handle(GetOrderByIdQueryData.ForId(order.Id), CancellationToken.None);

        result.ShouldBeSuccess();
        result.Data!.UserId.Should().Be(user.Id);
        result.Data!.Items.Should().ContainSingle();
    }

    [Fact]
    public async Task Returns_notfound_for_unknown_id()
    {
        using var db = TestDbContextFactory.Create();
        var handler = new GetOrderByIdQueryHandler(db);

        var result = await handler.Handle(GetOrderByIdQueryData.Unknown(), CancellationToken.None);

        result.ShouldBeFailureOfType(ResultErrorType.NotFound);
    }
}
