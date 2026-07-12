using MicroDemo.Domain.Entities;

namespace MicroDemo.Application.Features.Utenti.Dtos;

/// <summary>Projeção de leitura de <see cref="Utente"/>.</summary>
public class UtenteDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; }
    public Guid? DefaultPriceId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}

public static class UtenteMappingExtensions
{
    public static UtenteDto ToDto(this Utente utente) => new()
    {
        Id = utente.Id,
        FullName = utente.FullName,
        Email = utente.Email,
        PhoneNumber = utente.PhoneNumber,
        IsActive = utente.IsActive,
        DefaultPriceId = utente.DefaultPriceId,
        CreatedAtUtc = utente.CreatedAtUtc
    };
}
