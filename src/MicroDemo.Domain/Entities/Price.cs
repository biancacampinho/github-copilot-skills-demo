using MicroDemo.Domain.Common;
using MicroDemo.Domain.Enums;

namespace MicroDemo.Domain.Entities;

/// <summary>
/// Representa um preço/plano que pode ser associado a um <see cref="Utente"/>.
/// </summary>
public class Price : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Amount { get; set; }

    /// <summary>Código ISO-4217 (ex.: "EUR", "BRL", "USD").</summary>
    public string Currency { get; set; } = "EUR";

    public BillingPeriod BillingPeriod { get; set; } = BillingPeriod.Monthly;

    public bool IsActive { get; set; } = true;

    // Navegação
    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}
