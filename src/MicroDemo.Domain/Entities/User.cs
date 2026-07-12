using MicroDemo.Domain.Common;

namespace MicroDemo.Domain.Entities;

/// <summary>
/// Representa um utilizador (cliente/comprador) da loja. Pode ter um histórico
/// de pedidos (<see cref="Order"/>).
/// </summary>
public class User : BaseEntity
{
    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public bool IsActive { get; set; } = true;

    // Navegação
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
