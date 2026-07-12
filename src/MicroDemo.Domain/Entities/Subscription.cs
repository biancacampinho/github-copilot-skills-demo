using MicroDemo.Domain.Common;
using MicroDemo.Domain.Enums;

namespace MicroDemo.Domain.Entities;

/// <summary>
/// Associação temporal entre um <see cref="Utente"/> e um <see cref="Price"/> (plano).
/// É a entidade central do relacionamento utente↔price.
/// </summary>
public class Subscription : BaseEntity
{
    public Guid UtenteId { get; set; }
    public Utente Utente { get; set; } = null!;

    public Guid PriceId { get; set; }
    public Price Price { get; set; } = null!;

    public DateTime StartDateUtc { get; set; }

    public DateTime? EndDateUtc { get; set; }

    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Active;
}
