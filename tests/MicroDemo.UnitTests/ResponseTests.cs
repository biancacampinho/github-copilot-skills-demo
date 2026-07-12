using FluentAssertions;
using MicroDemo.Application.Common.Models;
using MicroDemo.Application.Features.Prices.Dtos;
using MicroDemo.Domain.Enums;
using Xunit;

namespace MicroDemo.UnitTests;

// ─────────────────────────────────────────────────────────────────────────────
//  ResponseTests.cs
//  Organização CUSTOMIZADA por TIPO DE ARTEFATO: este arquivo concentra os
//  BUILDERS de response/DTO esperados e helpers de asserção sobre o envelope
//  Result<T>, reutilizados por HandlerTests. Ter os "responses esperados" num
//  único lugar facilita comparar o retorno dos handlers de forma consistente.
// ─────────────────────────────────────────────────────────────────────────────

/// <summary>Builders de respostas/DTO esperados.</summary>
public static class PriceResponses
{
    public static PriceDto Expected() => new()
    {
        Name = "Plano Pro",
        Description = "Plano profissional mensal",
        Amount = 29.90m,
        Currency = "EUR",
        BillingPeriod = BillingPeriod.Monthly,
        IsActive = true
    };
}

/// <summary>Helpers de asserção sobre o envelope <see cref="Result"/>.</summary>
public static class ResultAssertions
{
    public static void ShouldBeSuccess(this Result result)
    {
        result.Succeeded.Should().BeTrue();
        result.Error.Should().BeNull();
    }

    public static void ShouldBeFailureOfType(this Result result, ResultErrorType type)
    {
        result.Succeeded.Should().BeFalse();
        result.ErrorType.Should().Be(type);
        result.Error.Should().NotBeNullOrWhiteSpace();
    }
}

public class ResponseBuildersSmokeTests
{
    [Fact]
    public void PriceResponses_Expected_matches_valid_request_shape()
    {
        var expected = PriceResponses.Expected();
        expected.Currency.Should().Be("EUR");
        expected.Amount.Should().Be(29.90m);
    }

    [Fact]
    public void ResultAssertions_detects_success()
    {
        Result.Success().ShouldBeSuccess();
    }

    [Fact]
    public void ResultAssertions_detects_notfound()
    {
        Result.NotFound("x").ShouldBeFailureOfType(ResultErrorType.NotFound);
    }
}
