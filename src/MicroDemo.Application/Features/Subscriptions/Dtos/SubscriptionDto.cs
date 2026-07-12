using MicroDemo.Domain.Entities;
using MicroDemo.Domain.Enums;

namespace MicroDemo.Application.Features.Subscriptions.Dtos;

/// <summary>Projeção de leitura de <see cref="Subscription"/>.</summary>
public class SubscriptionDto
{
    public Guid Id { get; set; }
    public Guid UtenteId { get; set; }
    public Guid PriceId { get; set; }
    public DateTime StartDateUtc { get; set; }
    public DateTime? EndDateUtc { get; set; }
    public SubscriptionStatus Status { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}

public static class SubscriptionMappingExtensions
{
    public static SubscriptionDto ToDto(this Subscription s) => new()
    {
        Id = s.Id,
        UtenteId = s.UtenteId,
        PriceId = s.PriceId,
        StartDateUtc = s.StartDateUtc,
        EndDateUtc = s.EndDateUtc,
        Status = s.Status,
        CreatedAtUtc = s.CreatedAtUtc
    };
}
