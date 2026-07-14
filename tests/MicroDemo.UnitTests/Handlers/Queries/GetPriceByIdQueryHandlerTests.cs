using FluentAssertions;
using MicroDemo.Application.Common.Models;
using MicroDemo.Application.Handlers.Queries.Prices;
using MicroDemo.Domain.Entities;
using MicroDemo.UnitTests.Common;
using MicroDemo.UnitTests.Queries;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MicroDemo.UnitTests.Handlers.Queries;

public class GetPriceByIdQueryHandlerTests
{
    [Fact]
    public async Task Returns_price_for_existing_id()
    {
        using var db = TestDbContextFactory.Create();
        var category = new Category { Name = "Electronics" };
        var product = new Product { Name = "Headphone", Sku = "SKU-A", Category = category };
        var price = new Price { Product = product, Amount = 29.90m, Currency = "EUR" };
        db.Categories.Add(category);
        db.Products.Add(product);
        db.Prices.Add(price);
        await db.SaveChangesAsync();

        var handler = new GetPriceByIdQueryHandler(db);
        var result = await handler.Handle(GetPriceByIdQueryData.ForId(price.Id), CancellationToken.None);

        result.ShouldBeSuccess();
        result.Data!.Amount.Should().Be(29.90m);
    }

    [Fact]
    public async Task Returns_notfound_for_unknown_id()
    {
        using var db = TestDbContextFactory.Create();
        var handler = new GetPriceByIdQueryHandler(db);

        var result = await handler.Handle(GetPriceByIdQueryData.Unknown(), CancellationToken.None);

        result.ShouldBeFailureOfType(ResultErrorType.NotFound);
    }
}
