using MicroDemo.Domain.Common;

namespace MicroDemo.Domain.Entities;

/// <summary>
/// Representa um utente (usuário/cliente). Pode possuir um plano/preço padrão
/// e um histórico de assinaturas e pedidos.
/// </summary>
public class Utente : BaseEntity
{
    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public bool IsActive { get; set; } = true;

    /// <summary>Plano/preço padrão associado ao utente (opcional).</summary>
    public Guid? DefaultPriceId { get; set; }
    public Price? DefaultPrice { get; set; }

    // Navegação
    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
