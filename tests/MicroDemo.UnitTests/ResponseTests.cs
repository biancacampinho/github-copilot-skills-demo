using FluentAssertions;
using MicroDemo.Application.Common.Models;
using MicroDemo.Application.Features.Products.Dtos;
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
public static class ProductResponses
{
    public static ProductDto Expected() => new()
    {
        Name = "Auscultadores Bluetooth",
        Description = "Auscultadores over-ear com cancelamento de ruído",
        Sku = "SKU-HEADPHONE-001",
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
    public void ProductResponses_Expected_matches_valid_request_shape()
    {
        var expected = ProductResponses.Expected();
        expected.Sku.Should().Be("SKU-HEADPHONE-001");
        expected.IsActive.Should().BeTrue();
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
