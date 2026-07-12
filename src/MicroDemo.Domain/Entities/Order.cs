using MicroDemo.Domain.Common;
using MicroDemo.Domain.Enums;

namespace MicroDemo.Domain.Entities;

/// <summary>
/// Pedido/cobrança gerado para um utente, tipicamente a partir de uma assinatura.
/// </summary>
public class Order : BaseEntity
{
    public Guid UtenteId { get; set; }
    public Utente Utente { get; set; } = null!;

    public Guid? SubscriptionId { get; set; }
    public Subscription? Subscription { get; set; }

    public decimal TotalAmount { get; set; }

    public string Currency { get; set; } = "EUR";

    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public DateTime OrderDateUtc { get; set; } = DateTime.UtcNow;
}
