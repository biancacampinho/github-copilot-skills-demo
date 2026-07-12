using MicroDemo.Domain.Entities;

namespace MicroDemo.Application.Features.Prices.Dtos;

/// <summary>Projeção de leitura de <see cref="Price"/> (preço de um produto).</summary>
public class PriceDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public DateTime ValidFromUtc { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}

public static class PriceMappingExtensions
{
    public static PriceDto ToDto(this Price price) => new()
    {
        Id = price.Id,
        ProductId = price.ProductId,
        Amount = price.Amount,
        Currency = price.Currency,
        ValidFromUtc = price.ValidFromUtc,
        IsActive = price.IsActive,
        CreatedAtUtc = price.CreatedAtUtc
    };
}
