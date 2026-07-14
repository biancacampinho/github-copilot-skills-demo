using FluentAssertions;
using MicroDemo.Application.Common.Models;
using MicroDemo.Application.Handlers.Commands.Prices;
using MicroDemo.Domain.Entities;
using MicroDemo.UnitTests.Commands;
using MicroDemo.UnitTests.Common;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MicroDemo.UnitTests.Handlers.Commands;

public class CreatePriceCommandHandlerTests
{
    [Fact]
    public async Task Persists_and_uppercases_currency()
    {
        using var db = TestDbContextFactory.Create();
        var category = new Category { Name = "Electronics" };
        var product = new Product { Name = "Headphone", Sku = "SKU-A", Category = category };
        db.Categories.Add(category);
        db.Products.Add(product);
        await db.SaveChangesAsync();

        var handler = new CreatePriceCommandHandler(db, TestLogger.For<CreatePriceCommandHandler>());
        var result = await handler.Handle(CreatePriceCommandData.Valid(product.Id) with { Currency = "eur" }, CancellationToken.None);

        result.ShouldBeSuccess();
        var price = await db.Prices.SingleAsync();
        price.Currency.Should().Be("EUR");
        price.Amount.Should().Be(29.90m);
    }

    [Fact]
    public async Task Returns_notfound_when_product_missing()
    {
        using var db = TestDbContextFactory.Create();
        var handler = new CreatePriceCommandHandler(db, TestLogger.For<CreatePriceCommandHandler>());

        var result = await handler.Handle(CreatePriceCommandData.Valid(Guid.NewGuid()), CancellationToken.None);

        result.ShouldBeFailureOfType(ResultErrorType.NotFound);
    }
}
