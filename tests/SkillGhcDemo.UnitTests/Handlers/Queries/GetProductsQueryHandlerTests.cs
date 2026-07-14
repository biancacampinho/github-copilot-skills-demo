using FluentAssertions;
using SkillGhcDemo.Application.Handlers.Queries.Products;
using SkillGhcDemo.Domain.Entities;
using SkillGhcDemo.UnitTests.Common;
using SkillGhcDemo.UnitTests.Queries;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace SkillGhcDemo.UnitTests.Handlers.Queries;

public class GetProductsQueryHandlerTests
{
    [Fact]
    public async Task OnlyActive_filters_inactive_products()
    {
        using var db = TestDbContextFactory.Create();
        var category = new Category { Name = "Electronics" };
        db.Categories.Add(category);
        db.Products.Add(new Product { Name = "Active", Sku = "SKU-ACTIVE", Category = category, IsActive = true });
        db.Products.Add(new Product { Name = "Inactive", Sku = "SKU-INACTIVE", Category = category, IsActive = false });
        await db.SaveChangesAsync();

        var handler = new GetProductsQueryHandler(db);
        var result = await handler.Handle(GetProductsQueryData.OnlyActive(), CancellationToken.None);

        result.ShouldBeSuccess();
        result.Data!.Should().ContainSingle().Which.Name.Should().Be("Active");
    }

    [Fact]
    public async Task Filters_by_category()
    {
        using var db = TestDbContextFactory.Create();
        var electronics = new Category { Name = "Electronics" };
        var books = new Category { Name = "Books" };
        db.Categories.AddRange(electronics, books);
        db.Products.Add(new Product { Name = "Headphone", Sku = "SKU-HEADPHONE", Category = electronics });
        db.Products.Add(new Product { Name = "Novel", Sku = "SKU-NOVEL", Category = books });
        await db.SaveChangesAsync();

        var handler = new GetProductsQueryHandler(db);
        var result = await handler.Handle(GetProductsQueryData.ByCategory(electronics.Id), CancellationToken.None);

        result.ShouldBeSuccess();
        result.Data!.Should().ContainSingle().Which.Name.Should().Be("Headphone");
    }
}
