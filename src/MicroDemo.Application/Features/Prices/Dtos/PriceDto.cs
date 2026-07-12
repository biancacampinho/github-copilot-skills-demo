using MicroDemo.Domain.Entities;
using MicroDemo.Domain.Enums;

namespace MicroDemo.Application.Features.Prices.Dtos;

/// <summary>Projeção de leitura de <see cref="Price"/>.</summary>
public class PriceDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public BillingPeriod BillingPeriod { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}

public static class PriceMappingExtensions
{
    public static PriceDto ToDto(this Price price) => new()
    {
        Id = price.Id,
        Name = price.Name,
        Description = price.Description,
        Amount = price.Amount,
        Currency = price.Currency,
        BillingPeriod = price.BillingPeriod,
        IsActive = price.IsActive,
        CreatedAtUtc = price.CreatedAtUtc
    };
}
