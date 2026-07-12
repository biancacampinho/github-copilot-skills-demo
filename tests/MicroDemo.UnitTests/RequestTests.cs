using FluentAssertions;
using MicroDemo.Application.Features.Prices.Commands.CreatePrice;
using MicroDemo.Application.Features.Subscriptions.Commands.CreateSubscription;
using MicroDemo.Application.Features.Utenti.Commands.CreateUtente;
using MicroDemo.Domain.Enums;
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

/// <summary>Builders de <see cref="CreatePriceCommand"/>.</summary>
public static class PriceRequests
{
    public static CreatePriceCommand Valid() => new()
    {
        Name = "Plano Pro",
        Description = "Plano profissional mensal",
        Amount = 29.90m,
        Currency = "EUR",
        BillingPeriod = BillingPeriod.Monthly,
        IsActive = true
    };

    public static CreatePriceCommand WithEmptyName() => Valid() with { Name = "" };

    public static CreatePriceCommand WithNegativeAmount() => Valid() with { Amount = -1m };

    public static CreatePriceCommand WithInvalidCurrency() => Valid() with { Currency = "EURO" };
}

/// <summary>Builders de <see cref="CreateUtenteCommand"/>.</summary>
public static class UtenteRequests
{
    public static CreateUtenteCommand Valid() => new()
    {
        FullName = "Mario Rossi",
        Email = "mario.rossi@example.com",
        PhoneNumber = "+39 333 1234567",
        DefaultPriceId = null
    };

    public static CreateUtenteCommand WithEmail(string email) => Valid() with { Email = email };

    public static CreateUtenteCommand WithInvalidEmail() => Valid() with { Email = "not-an-email" };

    public static CreateUtenteCommand WithEmptyName() => Valid() with { FullName = "" };
}

/// <summary>Builders de <see cref="CreateSubscriptionCommand"/>.</summary>
public static class SubscriptionRequests
{
    // Datas fixas para determinismo (sem DateTime.UtcNow nos testes).
    public static readonly DateTime Start = new(2026, 01, 01, 0, 0, 0, DateTimeKind.Utc);

    public static CreateSubscriptionCommand Valid() => new()
    {
        UtenteId = Guid.NewGuid(),
        PriceId = Guid.NewGuid(),
        StartDateUtc = Start,
        EndDateUtc = Start.AddMonths(1)
    };

    public static CreateSubscriptionCommand WithEndBeforeStart() =>
        Valid() with { EndDateUtc = Start.AddDays(-1) };

    public static CreateSubscriptionCommand WithEmptyUtente() =>
        Valid() with { UtenteId = Guid.Empty };
}

public class RequestBuildersSmokeTests
{
    [Fact]
    public void PriceRequests_Valid_produces_populated_command()
    {
        var cmd = PriceRequests.Valid();
        cmd.Name.Should().NotBeNullOrWhiteSpace();
        cmd.Amount.Should().BeGreaterThan(0);
        cmd.Currency.Should().HaveLength(3);
    }

    [Fact]
    public void UtenteRequests_Valid_produces_populated_command()
    {
        var cmd = UtenteRequests.Valid();
        cmd.FullName.Should().NotBeNullOrWhiteSpace();
        cmd.Email.Should().Contain("@");
    }

    [Fact]
    public void SubscriptionRequests_Valid_has_end_after_start()
    {
        var cmd = SubscriptionRequests.Valid();
        cmd.EndDateUtc.Should().BeAfter(cmd.StartDateUtc);
    }
}
