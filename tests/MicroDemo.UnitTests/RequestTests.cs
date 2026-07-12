using FluentAssertions;
using MicroDemo.Application.Features.Categories.Commands.CreateCategory;
using MicroDemo.Application.Features.Orders.Commands.CreateOrder;
using MicroDemo.Application.Features.Prices.Commands.CreatePrice;
using MicroDemo.Application.Features.Products.Commands.CreateProduct;
using MicroDemo.Application.Features.Users.Commands.CreateUser;
using Xunit;

namespace MicroDemo.UnitTests;

// ─────────────────────────────────────────────────────────────────────────────
//  RequestTests.cs
//  Organização CUSTOMIZADA por TIPO DE ARTEFATO: este arquivo concentra os
//  BUILDERS de request (commands/queries) reutilizados por HandlerTests e
//  ValidatorTests. Centralizar aqui evita duplicar a montagem de requests em
//  cada teste e dá um único ponto para ajustar os "dados válidos padrão".
//  As classes *Requests abaixo são os builders; os [Fact] ao final são apenas
//  smoke-tests que garantem que os builders continuam produzindo dados válidos.
// ─────────────────────────────────────────────────────────────────────────────

/// <summary>Builders de <see cref="CreateUserCommand"/>.</summary>
public static class UserRequests
{
    public static CreateUserCommand Valid() => new()
    {
        FullName = "Mario Rossi",
        Email = "mario.rossi@example.com",
        PhoneNumber = "+39 333 1234567"
    };

    public static CreateUserCommand WithEmail(string email) => Valid() with { Email = email };

    public static CreateUserCommand WithInvalidEmail() => Valid() with { Email = "not-an-email" };

    public static CreateUserCommand WithEmptyName() => Valid() with { FullName = "" };
}

/// <summary>Builders de <see cref="CreateCategoryCommand"/>.</summary>
public static class CategoryRequests
{
    public static CreateCategoryCommand Valid() => new()
    {
        Name = "Eletrónica",
        Description = "Dispositivos e acessórios eletrónicos",
        IsActive = true
    };

    public static CreateCategoryCommand WithName(string name) => Valid() with { Name = name };

    public static CreateCategoryCommand WithEmptyName() => Valid() with { Name = "" };
}

/// <summary>Builders de <see cref="CreateProductCommand"/>.</summary>
public static class ProductRequests
{
    public static CreateProductCommand Valid(Guid? categoryId = null) => new()
    {
        Name = "Auscultadores Bluetooth",
        Description = "Auscultadores over-ear com cancelamento de ruído",
        Sku = "SKU-HEADPHONE-001",
        CategoryId = categoryId ?? Guid.NewGuid(),
        IsActive = true
    };

    public static CreateProductCommand WithEmptyName(Guid? categoryId = null) =>
        Valid(categoryId) with { Name = "" };

    public static CreateProductCommand WithEmptySku(Guid? categoryId = null) =>
        Valid(categoryId) with { Sku = "" };

    public static CreateProductCommand WithEmptyCategory() =>
        Valid() with { CategoryId = Guid.Empty };
}

/// <summary>Builders de <see cref="CreatePriceCommand"/>.</summary>
public static class PriceRequests
{
    public static readonly DateTime ValidFrom = new(2026, 01, 01, 0, 0, 0, DateTimeKind.Utc);

    public static CreatePriceCommand Valid(Guid? productId = null) => new()
    {
        ProductId = productId ?? Guid.NewGuid(),
        Amount = 29.90m,
        Currency = "EUR",
        ValidFromUtc = ValidFrom,
        IsActive = true
    };

    public static CreatePriceCommand WithNegativeAmount(Guid? productId = null) =>
        Valid(productId) with { Amount = -1m };

    public static CreatePriceCommand WithInvalidCurrency(Guid? productId = null) =>
        Valid(productId) with { Currency = "EURO" };

    public static CreatePriceCommand WithEmptyProduct() =>
        Valid() with { ProductId = Guid.Empty };
}

/// <summary>Builders de <see cref="CreateOrderCommand"/>.</summary>
public static class OrderRequests
{
    public static CreateOrderCommand Valid(Guid? userId = null, Guid? productId = null) => new()
    {
        UserId = userId ?? Guid.NewGuid(),
        Items = new List<CreateOrderItem>
        {
            new() { ProductId = productId ?? Guid.NewGuid(), Quantity = 2 }
        }
    };

    public static CreateOrderCommand WithNoItems() =>
        Valid() with { Items = new List<CreateOrderItem>() };

    public static CreateOrderCommand WithZeroQuantity() =>
        Valid() with { Items = new List<CreateOrderItem> { new() { ProductId = Guid.NewGuid(), Quantity = 0 } } };

    public static CreateOrderCommand WithEmptyUser() =>
        Valid() with { UserId = Guid.Empty };
}

public class RequestBuildersSmokeTests
{
    [Fact]
    public void UserRequests_Valid_produces_populated_command()
    {
        var cmd = UserRequests.Valid();
        cmd.FullName.Should().NotBeNullOrWhiteSpace();
        cmd.Email.Should().Contain("@");
    }

    [Fact]
    public void CategoryRequests_Valid_produces_populated_command()
    {
        var cmd = CategoryRequests.Valid();
        cmd.Name.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void ProductRequests_Valid_produces_populated_command()
    {
        var cmd = ProductRequests.Valid();
        cmd.Name.Should().NotBeNullOrWhiteSpace();
        cmd.Sku.Should().NotBeNullOrWhiteSpace();
        cmd.CategoryId.Should().NotBeEmpty();
    }

    [Fact]
    public void PriceRequests_Valid_produces_populated_command()
    {
        var cmd = PriceRequests.Valid();
        cmd.ProductId.Should().NotBeEmpty();
        cmd.Amount.Should().BeGreaterThan(0);
        cmd.Currency.Should().HaveLength(3);
    }

    [Fact]
    public void OrderRequests_Valid_has_at_least_one_item()
    {
        var cmd = OrderRequests.Valid();
        cmd.UserId.Should().NotBeEmpty();
        cmd.Items.Should().NotBeEmpty();
        cmd.Items[0].Quantity.Should().BeGreaterThan(0);
    }
}
