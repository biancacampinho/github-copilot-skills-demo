using MicroDemo.Domain.Common;

namespace MicroDemo.Domain.Entities;

/// <summary>
/// Produto vendável. Pertence a uma <see cref="Category"/> e possui um histórico
/// de preços (<see cref="Price"/>). Aparece nos pedidos via <see cref="OrderItem"/>.
/// </summary>
public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    /// <summary>Código único do produto (Stock Keeping Unit).</summary>
    public string Sku { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    // Navegação
    /// <summary>Histórico de preços do produto (o preço "corrente" é o ativo mais recente).</summary>
    public ICollection<Price> Prices { get; set; } = new List<Price>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
