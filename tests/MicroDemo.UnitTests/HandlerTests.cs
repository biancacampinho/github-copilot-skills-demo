using FluentAssertions;
using MicroDemo.Application.Common.Models;
using MicroDemo.Application.Features.Categories.Commands.CreateCategory;
using MicroDemo.Application.Features.Categories.Commands.DeleteCategory;
using MicroDemo.Application.Features.Prices.Commands.CreatePrice;
using MicroDemo.Application.Features.Products.Commands.CreateProduct;
using MicroDemo.Application.Features.Products.Commands.DeleteProduct;
using MicroDemo.Application.Features.Products.Queries.GetProductById;
using MicroDemo.Application.Features.Products.Queries.GetProducts;
using MicroDemo.Application.Features.Users.Commands.CreateUser;
using MicroDemo.Application.Features.Users.Commands.DeleteUser;
using MicroDemo.Application.Features.Users.Queries.GetUserById;
using MicroDemo.Domain.Entities;
using MicroDemo.UnitTests.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace MicroDemo.UnitTests;

// ─────────────────────────────────────────────────────────────────────────────
//  HandlerTests.cs
//  Organização CUSTOMIZADA por TIPO DE ARTEFATO: TODOS os testes de handlers do
//  MediatR ficam neste arquivo, consumindo os builders de RequestTests.cs e os
//  helpers de asserção de ResponseTests.cs. Usa EF Core InMemory como repositório
//  e Moq para o ILogger.
// ─────────────────────────────────────────────────────────────────────────────

public class HandlerTests
{
    private static ILogger<T> Logger<T>() => new Mock<ILogger<T>>().Object;

    // ── Users ────────────────────────────────────────────────────────────────
    [Fact]
    public async Task CreateUser_succeeds_for_valid_request()
    {
        using var db = TestDbContextFactory.Create();
        var handler = new CreateUserCommandHandler(db, Logger<CreateUserCommandHandler>());

        var result = await handler.Handle(UserRequests.Valid(), CancellationToken.None);

        result.ShouldBeSuccess();
        (await db.Users.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task CreateUser_returns_conflict_for_duplicate_email()
    {
        using var db = TestDbContextFactory.Create();
        db.Users.Add(new User { FullName = "Existente", Email = "dup@example.com" });
        await db.SaveChangesAsync();

        var handler = new CreateUserCommandHandler(db, Logger<CreateUserCommandHandler>());
        var result = await handler.Handle(UserRequests.WithEmail("dup@example.com"), CancellationToken.None);

        result.ShouldBeFailureOfType(ResultErrorType.Conflict);
    }

    [Fact]
    public async Task DeleteUser_returns_conflict_when_orders_exist()
    {
        using var db = TestDbContextFactory.Create();
        var user = new User { FullName = "Com pedido", Email = "u@example.com" };
        db.Users.Add(user);
        db.Orders.Add(new Order { User = user, TotalAmount = 10m, Currency = "EUR" });
        await db.SaveChangesAsync();

        var handler = new DeleteUserCommandHandler(db, Logger<DeleteUserCommandHandler>());
        var result = await handler.Handle(new DeleteUserCommand(user.Id), CancellationToken.None);

        result.ShouldBeFailureOfType(ResultErrorType.Conflict);
    }

    [Fact]
    public async Task GetUserById_returns_notfound_for_unknown_id()
    {
        using var db = TestDbContextFactory.Create();
        var handler = new GetUserByIdQueryHandler(db);

        var result = await handler.Handle(new GetUserByIdQuery(Guid.NewGuid()), CancellationToken.None);

        result.ShouldBeFailureOfType(ResultErrorType.NotFound);
    }

    // ── Categories ─────────────────────────────────────────────────────────────
    [Fact]
    public async Task CreateCategory_succeeds_for_valid_request()
    {
        using var db = TestDbContextFactory.Create();
        var handler = new CreateCategoryCommandHandler(db, Logger<CreateCategoryCommandHandler>());

        var result = await handler.Handle(CategoryRequests.Valid(), CancellationToken.None);

        result.ShouldBeSuccess();
        (await db.Categories.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task CreateCategory_returns_conflict_for_duplicate_name()
    {
        using var db = TestDbContextFactory.Create();
        db.Categories.Add(new Category { Name = "Eletrónica" });
        await db.SaveChangesAsync();

        var handler = new CreateCategoryCommandHandler(db, Logger<CreateCategoryCommandHandler>());
        var result = await handler.Handle(CategoryRequests.WithName("Eletrónica"), CancellationToken.None);

        result.ShouldBeFailureOfType(ResultErrorType.Conflict);
    }

    [Fact]
    public async Task DeleteCategory_returns_conflict_when_products_exist()
    {
        using var db = TestDbContextFactory.Create();
        var category = new Category { Name = "Com produtos" };
        db.Categories.Add(category);
        db.Products.Add(new Product { Name = "P", Sku = "SKU-1", Category = category });
        await db.SaveChangesAsync();

        var handler = new DeleteCategoryCommandHandler(db, Logger<DeleteCategoryCommandHandler>());
        var result = await handler.Handle(new DeleteCategoryCommand(category.Id), CancellationToken.None);

        result.ShouldBeFailureOfType(ResultErrorType.Conflict);
    }

    // ── Products ───────────────────────────────────────────────────────────────
    [Fact]
    public async Task CreateProduct_succeeds_when_category_exists()
    {
        using var db = TestDbContextFactory.Create();
        var category = new Category { Name = "Eletrónica" };
        db.Categories.Add(category);
        await db.SaveChangesAsync();

        var handler = new CreateProductCommandHandler(db, Logger<CreateProductCommandHandler>());
        var result = await handler.Handle(ProductRequests.Valid(category.Id), CancellationToken.None);

        result.ShouldBeSuccess();
        (await db.Products.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task CreateProduct_returns_notfound_when_category_missing()
    {
        using var db = TestDbContextFactory.Create();
        var handler = new CreateProductCommandHandler(db, Logger<CreateProductCommandHandler>());

        var result = await handler.Handle(ProductRequests.Valid(Guid.NewGuid()), CancellationToken.None);

        result.ShouldBeFailureOfType(ResultErrorType.NotFound);
    }

    [Fact]
    public async Task CreateProduct_returns_conflict_for_duplicate_sku()
    {
        using var db = TestDbContextFactory.Create();
        var category = new Category { Name = "Eletrónica" };
        db.Categories.Add(category);
        db.Products.Add(new Product { Name = "Existente", Sku = "SKU-DUP", Category = category });
        await db.SaveChangesAsync();

        var handler = new CreateProductCommandHandler(db, Logger<CreateProductCommandHandler>());
        var request = ProductRequests.Valid(category.Id) with { Sku = "SKU-DUP" };
        var result = await handler.Handle(request, CancellationToken.None);

        result.ShouldBeFailureOfType(ResultErrorType.Conflict);
    }

    [Fact]
    public async Task GetProducts_onlyActive_filters_inactive()
    {
        using var db = TestDbContextFactory.Create();
        var category = new Category { Name = "Eletrónica" };
        db.Categories.Add(category);
        db.Products.Add(new Product { Name = "Ativo", Sku = "SKU-A", Category = category, IsActive = true });
        db.Products.Add(new Product { Name = "Inativo", Sku = "SKU-I", Category = category, IsActive = false });
        await db.SaveChangesAsync();

        var handler = new GetProductsQueryHandler(db);
        var result = await handler.Handle(new GetProductsQuery(OnlyActive: true), CancellationToken.None);

        result.ShouldBeSuccess();
        result.Data!.Should().ContainSingle().Which.Name.Should().Be("Ativo");
    }

    [Fact]
    public async Task GetProducts_filters_by_category()
    {
        using var db = TestDbContextFactory.Create();
        var electronics = new Category { Name = "Eletrónica" };
        var books = new Category { Name = "Livros" };
        db.Categories.AddRange(electronics, books);
        db.Products.Add(new Product { Name = "Headphone", Sku = "SKU-A", Category = electronics });
        db.Products.Add(new Product { Name = "Romance", Sku = "SKU-B", Category = books });
        await db.SaveChangesAsync();

        var handler = new GetProductsQueryHandler(db);
        var result = await handler.Handle(new GetProductsQuery(CategoryId: books.Id), CancellationToken.None);

        result.ShouldBeSuccess();
        result.Data!.Should().ContainSingle().Which.Name.Should().Be("Romance");
    }

    [Fact]
    public async Task GetProductById_exposes_current_active_price()
    {
        using var db = TestDbContextFactory.Create();
        var category = new Category { Name = "Eletrónica" };
        var product = new Product { Name = "Headphone", Sku = "SKU-A", Category = category };
        db.Categories.Add(category);
        db.Products.Add(product);
        db.Prices.Add(new Price { Product = product, Amount = 10m, Currency = "EUR", IsActive = true, ValidFromUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) });
        db.Prices.Add(new Price { Product = product, Amount = 15m, Currency = "EUR", IsActive = true, ValidFromUtc = new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc) });
        await db.SaveChangesAsync();

        var handler = new GetProductByIdQueryHandler(db);
        var result = await handler.Handle(new GetProductByIdQuery(product.Id), CancellationToken.None);

        result.ShouldBeSuccess();
        result.Data!.CurrentPrice.Should().Be(15m);
    }

    [Fact]
    public async Task DeleteProduct_returns_conflict_when_order_items_exist()
    {
        using var db = TestDbContextFactory.Create();
        var category = new Category { Name = "Eletrónica" };
        var product = new Product { Name = "Headphone", Sku = "SKU-A", Category = category };
        var user = new User { FullName = "U", Email = "u@example.com" };
        var order = new Order { User = user, TotalAmount = 20m, Currency = "EUR" };
        db.Categories.Add(category);
        db.Products.Add(product);
        db.Users.Add(user);
        db.Orders.Add(order);
        db.OrderItems.Add(new OrderItem { Order = order, Product = product, Quantity = 2, UnitPrice = 10m, Currency = "EUR", LineTotal = 20m });
        await db.SaveChangesAsync();

        var handler = new DeleteProductCommandHandler(db, Logger<DeleteProductCommandHandler>());
        var result = await handler.Handle(new DeleteProductCommand(product.Id), CancellationToken.None);

        result.ShouldBeFailureOfType(ResultErrorType.Conflict);
    }

    // ── Prices ─────────────────────────────────────────────────────────────────
    [Fact]
    public async Task CreatePrice_persists_and_uppercases_currency()
    {
        using var db = TestDbContextFactory.Create();
        var category = new Category { Name = "Eletrónica" };
        var product = new Product { Name = "Headphone", Sku = "SKU-A", Category = category };
        db.Categories.Add(category);
        db.Products.Add(product);
        await db.SaveChangesAsync();

        var handler = new CreatePriceCommandHandler(db, Logger<CreatePriceCommandHandler>());
        var request = PriceRequests.Valid(product.Id) with { Currency = "eur" };
        var result = await handler.Handle(request, CancellationToken.None);

        result.ShouldBeSuccess();
        var saved = await db.Prices.SingleAsync();
        saved.Currency.Should().Be("EUR");
        saved.Amount.Should().Be(29.90m);
    }

    [Fact]
    public async Task CreatePrice_returns_notfound_when_product_missing()
    {
        using var db = TestDbContextFactory.Create();
        var handler = new CreatePriceCommandHandler(db, Logger<CreatePriceCommandHandler>());

        var result = await handler.Handle(PriceRequests.Valid(Guid.NewGuid()), CancellationToken.None);

        result.ShouldBeFailureOfType(ResultErrorType.NotFound);
    }
}
