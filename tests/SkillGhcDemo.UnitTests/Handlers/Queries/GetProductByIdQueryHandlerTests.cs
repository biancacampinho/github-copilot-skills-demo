using FluentAssertions;
using SkillGhcDemo.Application.Common.Models;
using SkillGhcDemo.Application.Handlers.Queries.Products;
using SkillGhcDemo.Domain.Entities;
using SkillGhcDemo.UnitTests.Common;
using SkillGhcDemo.UnitTests.Queries;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace SkillGhcDemo.UnitTests.Handlers.Queries;

public class GetProductByIdQueryHandlerTests
{
    [Fact]
    public async Task Exposes_current_active_price()
    {
        using var db = TestDbContextFactory.Create();
        var category = new Category { Name = "Electronics" };
        var product = new Product { Name = "Headphone", Sku = "SKU-HEADPHONE", Category = category };
        db.Categories.Add(category);
        db.Products.Add(product);
        db.Prices.Add(new Price { Product = product, Amount = 10m, Currency = "EUR", IsActive = true, ValidFromUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) });
        db.Prices.Add(new Price { Product = product, Amount = 15m, Currency = "EUR", IsActive = true, ValidFromUtc = new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc) });
        await db.SaveChangesAsync();

        var handler = new GetProductByIdQueryHandler(db);
        var result = await handler.Handle(GetProductByIdQueryData.ForId(product.Id), CancellationToken.None);

        result.ShouldBeSuccess();
        result.Data!.CurrentPrice.Should().Be(15m);
    }

    [Fact]
    public async Task Returns_notfound_for_unknown_id()
    {
        using var db = TestDbContextFactory.Create();
        var handler = new GetProductByIdQueryHandler(db);

        var result = await handler.Handle(GetProductByIdQueryData.Unknown(), CancellationToken.None);

        result.ShouldBeFailureOfType(ResultErrorType.NotFound);
    }
}
