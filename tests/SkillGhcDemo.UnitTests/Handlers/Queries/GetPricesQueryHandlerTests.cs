using FluentAssertions;
using SkillGhcDemo.Application.Handlers.Queries.Prices;
using SkillGhcDemo.Domain.Entities;
using SkillGhcDemo.UnitTests.Common;
using SkillGhcDemo.UnitTests.Queries;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace SkillGhcDemo.UnitTests.Handlers.Queries;

public class GetPricesQueryHandlerTests
{
    [Fact]
    public async Task Returns_all_prices()
    {
        using var db = TestDbContextFactory.Create();
        var category = new Category { Name = "Electronics" };
        var product = new Product { Name = "Headphone", Sku = "SKU-A", Category = category };
        db.Categories.Add(category);
        db.Products.Add(product);
        db.Prices.AddRange(
            new Price { Product = product, Amount = 29.90m, Currency = "EUR" },
            new Price { Product = product, Amount = 19.90m, Currency = "EUR" });
        await db.SaveChangesAsync();

        var handler = new GetPricesQueryHandler(db);
        var result = await handler.Handle(GetPricesQueryData.All(), CancellationToken.None);

        result.ShouldBeSuccess();
        result.Data!.Should().HaveCount(2);
    }

    [Fact]
    public async Task OnlyActive_filters_inactive_prices()
    {
        using var db = TestDbContextFactory.Create();
        var category = new Category { Name = "Electronics" };
        var product = new Product { Name = "Headphone", Sku = "SKU-A", Category = category };
        db.Categories.Add(category);
        db.Products.Add(product);
        db.Prices.Add(new Price { Product = product, Amount = 29.90m, Currency = "EUR", IsActive = true });
        db.Prices.Add(new Price { Product = product, Amount = 19.90m, Currency = "EUR", IsActive = false });
        await db.SaveChangesAsync();

        var handler = new GetPricesQueryHandler(db);
        var result = await handler.Handle(GetPricesQueryData.OnlyActive(), CancellationToken.None);

        result.ShouldBeSuccess();
        result.Data!.Should().ContainSingle().Which.Amount.Should().Be(29.90m);
    }

    [Fact]
    public async Task ByProduct_filters_by_product()
    {
        using var db = TestDbContextFactory.Create();
        var category = new Category { Name = "Electronics" };
        var productA = new Product { Name = "Headphone", Sku = "SKU-A", Category = category };
        var productB = new Product { Name = "Keyboard", Sku = "SKU-B", Category = category };
        db.Categories.Add(category);
        db.Products.AddRange(productA, productB);
        db.Prices.Add(new Price { Product = productA, Amount = 29.90m, Currency = "EUR" });
        db.Prices.Add(new Price { Product = productB, Amount = 49.90m, Currency = "EUR" });
        await db.SaveChangesAsync();

        var handler = new GetPricesQueryHandler(db);
        var result = await handler.Handle(GetPricesQueryData.ByProduct(productA.Id), CancellationToken.None);

        result.ShouldBeSuccess();
        result.Data!.Should().ContainSingle().Which.ProductId.Should().Be(productA.Id);
    }
}
