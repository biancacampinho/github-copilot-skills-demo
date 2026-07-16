using FluentAssertions;
using SkillGhcDemo.Application.Handlers.Queries.Orders;
using SkillGhcDemo.Domain.Entities;
using SkillGhcDemo.UnitTests.Common;
using SkillGhcDemo.UnitTests.Queries;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace SkillGhcDemo.UnitTests.Handlers.Queries;

public class GetOrdersByProductIdQueryHandlerTests
{
    [Fact]
    public async Task Returns_orders_containing_the_product_with_full_user_and_product_details()
    {
        using var db = TestDbContextFactory.Create();

        var category = new Category { Name = "Electronics" };
        var user = new User { FullName = "Ada Lovelace", Email = "ada@example.com" };
        var matchingProduct = new Product { Name = "Keyboard", Sku = "KB-001", Category = category };
        var otherProduct = new Product { Name = "Mouse", Sku = "MS-001", Category = category };
        db.Categories.Add(category);
        db.Users.Add(user);
        db.Products.AddRange(matchingProduct, otherProduct);
        await db.SaveChangesAsync();

        var matchingOrder = new Order
        {
            UserId = user.Id,
            Currency = "EUR",
            TotalAmount = 40m,
            Items = new List<OrderItem>
            {
                new()
                {
                    ProductId = matchingProduct.Id,
                    Quantity = 2,
                    UnitPrice = 20m,
                    Currency = "EUR",
                    LineTotal = 40m
                }
            }
        };
        var otherOrder = new Order
        {
            UserId = user.Id,
            Currency = "EUR",
            TotalAmount = 15m,
            Items = new List<OrderItem>
            {
                new()
                {
                    ProductId = otherProduct.Id,
                    Quantity = 1,
                    UnitPrice = 15m,
                    Currency = "EUR",
                    LineTotal = 15m
                }
            }
        };
        db.Orders.AddRange(matchingOrder, otherOrder);
        await db.SaveChangesAsync();

        var handler = new GetOrdersByProductIdQueryHandler(db);
        var result = await handler.Handle(GetOrdersByProductIdQueryData.ForProduct(matchingProduct.Id), CancellationToken.None);

        result.ShouldBeSuccess();
        result.Data!.Should().ContainSingle().Which.Id.Should().Be(matchingOrder.Id);
        var dto = result.Data!.Single();
        dto.User.Email.Should().Be(user.Email);
        dto.Items.Should().ContainSingle().Which.Product.Name.Should().Be(matchingProduct.Name);
    }

    [Fact]
    public async Task Returns_empty_list_when_no_order_contains_the_product()
    {
        using var db = TestDbContextFactory.Create();
        var handler = new GetOrdersByProductIdQueryHandler(db);

        var result = await handler.Handle(GetOrdersByProductIdQueryData.Unknown(), CancellationToken.None);

        result.ShouldBeSuccess();
        result.Data!.Should().BeEmpty();
    }
}
