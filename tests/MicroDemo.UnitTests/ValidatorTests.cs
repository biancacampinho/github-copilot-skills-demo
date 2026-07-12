using FluentAssertions;
using FluentValidation.TestHelper;
using MicroDemo.Application.Features.Prices.Commands.CreatePrice;
using MicroDemo.Application.Features.Subscriptions.Commands.CreateSubscription;
using MicroDemo.Application.Features.Utenti.Commands.CreateUtente;
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
    private readonly CreatePriceCommandValidator _priceValidator = new();
    private readonly CreateUtenteCommandValidator _utenteValidator = new();
    private readonly CreateSubscriptionCommandValidator _subscriptionValidator = new();

    // ── CreatePrice ────────────────────────────────────────────────────────────
    [Fact]
    public void CreatePrice_valid_request_passes()
    {
        var result = _priceValidator.TestValidate(PriceRequests.Valid());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreatePrice_empty_name_fails()
    {
        var result = _priceValidator.TestValidate(PriceRequests.WithEmptyName());
        result.ShouldHaveValidationErrorFor(x => x.Name);
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

    // ── CreateUtente ───────────────────────────────────────────────────────────
    [Fact]
    public void CreateUtente_valid_request_passes()
    {
        var result = _utenteValidator.TestValidate(UtenteRequests.Valid());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreateUtente_invalid_email_fails()
    {
        var result = _utenteValidator.TestValidate(UtenteRequests.WithInvalidEmail());
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void CreateUtente_empty_name_fails()
    {
        var result = _utenteValidator.TestValidate(UtenteRequests.WithEmptyName());
        result.ShouldHaveValidationErrorFor(x => x.FullName);
    }

    // ── CreateSubscription (shape) ──────────────────────────────────────────────
    [Fact]
    public void CreateSubscription_valid_request_passes()
    {
        var result = _subscriptionValidator.TestValidate(SubscriptionRequests.Valid());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreateSubscription_end_before_start_fails()
    {
        var result = _subscriptionValidator.TestValidate(SubscriptionRequests.WithEndBeforeStart());
        result.ShouldHaveValidationErrorFor(x => x.EndDateUtc);
    }

    [Fact]
    public void CreateSubscription_empty_utente_fails()
    {
        var result = _subscriptionValidator.TestValidate(SubscriptionRequests.WithEmptyUtente());
        result.ShouldHaveValidationErrorFor(x => x.UtenteId);
    }
}
