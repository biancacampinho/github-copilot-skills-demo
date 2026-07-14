using FluentAssertions;
using MicroDemo.Application.Common.Models;
using MicroDemo.Application.Handlers.Commands.Prices;
using MicroDemo.Domain.Entities;
using MicroDemo.UnitTests.Commands;
using MicroDemo.UnitTests.Common;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MicroDemo.UnitTests.Handlers.Commands;

public class DeletePriceCommandHandlerTests
{
    [Fact]
    public async Task Succeeds_for_existing_price()
    {
        using var db = TestDbContextFactory.Create();
        var category = new Category { Name = "Electronics" };
        var product = new Product { Name = "Headphone", Sku = "SKU-A", Category = category };
        var price = new Price { Product = product, Amount = 29.90m, Currency = "EUR", IsActive = true };
        db.Categories.Add(category);
        db.Products.Add(product);
        db.Prices.Add(price);
        await db.SaveChangesAsync();

        var handler = new DeletePriceCommandHandler(db, TestLogger.For<DeletePriceCommandHandler>());
        var result = await handler.Handle(DeletePriceCommandData.ForId(price.Id), CancellationToken.None);

        result.ShouldBeSuccess();
        (await db.Prices.CountAsync()).Should().Be(0);
    }

    [Fact]
    public async Task Returns_notfound_for_unknown_id()
    {
        using var db = TestDbContextFactory.Create();
        var handler = new DeletePriceCommandHandler(db, TestLogger.For<DeletePriceCommandHandler>());

        var result = await handler.Handle(DeletePriceCommandData.Unknown(), CancellationToken.None);

        result.ShouldBeFailureOfType(ResultErrorType.NotFound);
    }
}
