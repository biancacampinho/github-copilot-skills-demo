using MicroDemo.Domain.Common;

namespace MicroDemo.Domain.Entities;

/// <summary>
/// Preço de um <see cref="Product"/>. Modelado como histórico (1:N com Product):
/// vários registos ao longo do tempo, sendo o preço "corrente" o ativo com o
/// <see cref="ValidFromUtc"/> mais recente.
/// </summary>
public class Price : BaseEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public decimal Amount { get; set; }

    /// <summary>Código ISO-4217 (ex.: "EUR", "BRL", "USD").</summary>
    public string Currency { get; set; } = "EUR";

    /// <summary>A partir de quando este preço passa a valer.</summary>
    public DateTime ValidFromUtc { get; set; } = DateTime.UtcNow;

    /// <summary>Indica se este é um preço atualmente em vigor (não descontinuado).</summary>
    public bool IsActive { get; set; } = true;
}
