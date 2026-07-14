using FluentAssertions;
using SkillGhcDemo.Application.Common.Models;
using SkillGhcDemo.Application.Handlers.Commands.Prices;
using SkillGhcDemo.Domain.Entities;
using SkillGhcDemo.UnitTests.Commands;
using SkillGhcDemo.UnitTests.Common;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace SkillGhcDemo.UnitTests.Handlers.Commands;

public class UpdatePriceCommandHandlerTests
{
    [Fact]
    public async Task Succeeds_for_existing_price()
    {
        using var db = TestDbContextFactory.Create();
        var category = new Category { Name = "Electronics" };
        var product = new Product { Name = "Headphone", Sku = "SKU-A", Category = category };
        var price = new Price { Product = product, Amount = 10m, Currency = "USD", IsActive = true };
        db.Categories.Add(category);
        db.Products.Add(product);
        db.Prices.Add(price);
        await db.SaveChangesAsync();

        var handler = new UpdatePriceCommandHandler(db, TestLogger.For<UpdatePriceCommandHandler>());
        var command = UpdatePriceCommandData.Valid(price.Id) with { Currency = "eur" };
        var result = await handler.Handle(command, CancellationToken.None);

        result.ShouldBeSuccess();
        var updated = await db.Prices.SingleAsync();
        updated.Amount.Should().Be(29.90m);
        updated.Currency.Should().Be("EUR");
    }

    [Fact]
    public async Task Returns_notfound_for_unknown_id()
    {
        using var db = TestDbContextFactory.Create();
        var handler = new UpdatePriceCommandHandler(db, TestLogger.For<UpdatePriceCommandHandler>());

        var result = await handler.Handle(UpdatePriceCommandData.Valid(Guid.NewGuid()), CancellationToken.None);

        result.ShouldBeFailureOfType(ResultErrorType.NotFound);
    }
}
