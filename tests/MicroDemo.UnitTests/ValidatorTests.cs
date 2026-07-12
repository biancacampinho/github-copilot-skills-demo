using FluentValidation.TestHelper;
using MicroDemo.Application.Features.Categories.Commands.CreateCategory;
using MicroDemo.Application.Features.Orders.Commands.CreateOrder;
using MicroDemo.Application.Features.Prices.Commands.CreatePrice;
using MicroDemo.Application.Features.Products.Commands.CreateProduct;
using MicroDemo.Application.Features.Users.Commands.CreateUser;
using Xunit;

namespace MicroDemo.UnitTests;

// ─────────────────────────────────────────────────────────────────────────────
//  ValidatorTests.cs
//  Organização CUSTOMIZADA por TIPO DE ARTEFATO: TODOS os testes de validadores
//  (FluentValidation) ficam neste arquivo, reutilizando os builders de
//  RequestTests.cs. Usa a API TestValidate() do FluentValidation.
// ─────────────────────────────────────────────────────────────────────────────

public class ValidatorTests
{
    private readonly CreateUserCommandValidator _userValidator = new();
    private readonly CreateCategoryCommandValidator _categoryValidator = new();
    private readonly CreateProductCommandValidator _productValidator = new();
    private readonly CreatePriceCommandValidator _priceValidator = new();
    private readonly CreateOrderCommandValidator _orderValidator = new();

    // ── CreateUser ─────────────────────────────────────────────────────────────
    [Fact]
    public void CreateUser_valid_request_passes()
    {
        var result = _userValidator.TestValidate(UserRequests.Valid());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreateUser_invalid_email_fails()
    {
        var result = _userValidator.TestValidate(UserRequests.WithInvalidEmail());
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void CreateUser_empty_name_fails()
    {
        var result = _userValidator.TestValidate(UserRequests.WithEmptyName());
        result.ShouldHaveValidationErrorFor(x => x.FullName);
    }

    // ── CreateCategory ──────────────────────────────────────────────────────────
    [Fact]
    public void CreateCategory_valid_request_passes()
    {
        var result = _categoryValidator.TestValidate(CategoryRequests.Valid());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreateCategory_empty_name_fails()
    {
        var result = _categoryValidator.TestValidate(CategoryRequests.WithEmptyName());
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    // ── CreateProduct ───────────────────────────────────────────────────────────
    [Fact]
    public void CreateProduct_valid_request_passes()
    {
        var result = _productValidator.TestValidate(ProductRequests.Valid());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreateProduct_empty_name_fails()
    {
        var result = _productValidator.TestValidate(ProductRequests.WithEmptyName());
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void CreateProduct_empty_sku_fails()
    {
        var result = _productValidator.TestValidate(ProductRequests.WithEmptySku());
        result.ShouldHaveValidationErrorFor(x => x.Sku);
    }

    [Fact]
    public void CreateProduct_empty_category_fails()
    {
        var result = _productValidator.TestValidate(ProductRequests.WithEmptyCategory());
        result.ShouldHaveValidationErrorFor(x => x.CategoryId);
    }

    // ── CreatePrice ─────────────────────────────────────────────────────────────
    [Fact]
    public void CreatePrice_valid_request_passes()
    {
        var result = _priceValidator.TestValidate(PriceRequests.Valid());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreatePrice_negative_amount_fails()
    {
        var result = _priceValidator.TestValidate(PriceRequests.WithNegativeAmount());
        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }

    [Fact]
    public void CreatePrice_invalid_currency_length_fails()
    {
        var result = _priceValidator.TestValidate(PriceRequests.WithInvalidCurrency());
        result.ShouldHaveValidationErrorFor(x => x.Currency);
    }

    [Fact]
    public void CreatePrice_empty_product_fails()
    {
        var result = _priceValidator.TestValidate(PriceRequests.WithEmptyProduct());
        result.ShouldHaveValidationErrorFor(x => x.ProductId);
    }

    // ── CreateOrder (shape) ─────────────────────────────────────────────────────
    [Fact]
    public void CreateOrder_valid_request_passes()
    {
        var result = _orderValidator.TestValidate(OrderRequests.Valid());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreateOrder_no_items_fails()
    {
        var result = _orderValidator.TestValidate(OrderRequests.WithNoItems());
        result.ShouldHaveValidationErrorFor(x => x.Items);
    }

    [Fact]
    public void CreateOrder_zero_quantity_fails()
    {
        var result = _orderValidator.TestValidate(OrderRequests.WithZeroQuantity());
        result.ShouldHaveValidationErrorFor("Items[0].Quantity");
    }

    [Fact]
    public void CreateOrder_empty_user_fails()
    {
        var result = _orderValidator.TestValidate(OrderRequests.WithEmptyUser());
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }
}
