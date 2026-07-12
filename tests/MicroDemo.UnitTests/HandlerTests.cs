using FluentAssertions;
using MicroDemo.Application.Common.Models;
using MicroDemo.Application.Features.Prices.Commands.CreatePrice;
using MicroDemo.Application.Features.Prices.Commands.DeletePrice;
using MicroDemo.Application.Features.Prices.Queries.GetPrices;
using MicroDemo.Application.Features.Utenti.Commands.CreateUtente;
using MicroDemo.Application.Features.Utenti.Queries.GetUtenteById;
using MicroDemo.Domain.Entities;
using MicroDemo.Domain.Enums;
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

    [Fact]
    public async Task CreatePrice_persists_and_returns_id_with_uppercased_currency()
    {
        using var db = TestDbContextFactory.Create();
        var handler = new CreatePriceCommandHandler(db, Logger<CreatePriceCommandHandler>());
        var request = PriceRequests.Valid() with { Currency = "eur" };

        var result = await handler.Handle(request, CancellationToken.None);

        result.ShouldBeSuccess();
        result.Data.Should().NotBeEmpty();

        var saved = await db.Prices.SingleAsync();
        saved.Currency.Should().Be("EUR");
        saved.Name.Should().Be("Plano Pro");
    }

    [Fact]
    public async Task CreateUtente_succeeds_for_valid_request()
    {
        using var db = TestDbContextFactory.Create();
        var handler = new CreateUtenteCommandHandler(db, Logger<CreateUtenteCommandHandler>());

        var result = await handler.Handle(UtenteRequests.Valid(), CancellationToken.None);

        result.ShouldBeSuccess();
        (await db.Utenti.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task CreateUtente_returns_conflict_for_duplicate_email()
    {
        using var db = TestDbContextFactory.Create();
        db.Utenti.Add(new Utente { FullName = "Existente", Email = "dup@example.com" });
        await db.SaveChangesAsync();

        var handler = new CreateUtenteCommandHandler(db, Logger<CreateUtenteCommandHandler>());
        var result = await handler.Handle(UtenteRequests.WithEmail("dup@example.com"), CancellationToken.None);

        result.ShouldBeFailureOfType(ResultErrorType.Conflict);
    }

    [Fact]
    public async Task CreateUtente_returns_notfound_when_default_price_missing()
    {
        using var db = TestDbContextFactory.Create();
        var handler = new CreateUtenteCommandHandler(db, Logger<CreateUtenteCommandHandler>());
        var request = UtenteRequests.Valid() with { DefaultPriceId = Guid.NewGuid() };

        var result = await handler.Handle(request, CancellationToken.None);

        result.ShouldBeFailureOfType(ResultErrorType.NotFound);
    }

    [Fact]
    public async Task GetPrices_onlyActive_filters_inactive()
    {
        using var db = TestDbContextFactory.Create();
        db.Prices.Add(new Price { Name = "Ativo", Amount = 10, Currency = "EUR", IsActive = true });
        db.Prices.Add(new Price { Name = "Inativo", Amount = 20, Currency = "EUR", IsActive = false });
        await db.SaveChangesAsync();

        var handler = new GetPricesQueryHandler(db);
        var result = await handler.Handle(new GetPricesQuery(OnlyActive: true), CancellationToken.None);

        result.ShouldBeSuccess();
        result.Data!.Should().ContainSingle().Which.Name.Should().Be("Ativo");
    }

    [Fact]
    public async Task DeletePrice_returns_conflict_when_subscriptions_exist()
    {
        using var db = TestDbContextFactory.Create();
        var price = new Price { Name = "Com assinatura", Amount = 10, Currency = "EUR" };
        var utente = new Utente { FullName = "U", Email = "u@example.com" };
        db.Prices.Add(price);
        db.Utenti.Add(utente);
        db.Subscriptions.Add(new Subscription
        {
            Price = price,
            Utente = utente,
            StartDateUtc = SubscriptionRequests.Start,
            Status = SubscriptionStatus.Active
        });
        await db.SaveChangesAsync();

        var handler = new DeletePriceCommandHandler(db, Logger<DeletePriceCommandHandler>());
        var result = await handler.Handle(new DeletePriceCommand(price.Id), CancellationToken.None);

        result.ShouldBeFailureOfType(ResultErrorType.Conflict);
    }

    [Fact]
    public async Task DeletePrice_returns_notfound_for_unknown_id()
    {
        using var db = TestDbContextFactory.Create();
        var handler = new DeletePriceCommandHandler(db, Logger<DeletePriceCommandHandler>());

        var result = await handler.Handle(new DeletePriceCommand(Guid.NewGuid()), CancellationToken.None);

        result.ShouldBeFailureOfType(ResultErrorType.NotFound);
    }

    [Fact]
    public async Task GetUtenteById_returns_notfound_for_unknown_id()
    {
        using var db = TestDbContextFactory.Create();
        var handler = new GetUtenteByIdQueryHandler(db);

        var result = await handler.Handle(new GetUtenteByIdQuery(Guid.NewGuid()), CancellationToken.None);

        result.ShouldBeFailureOfType(ResultErrorType.NotFound);
    }
}
