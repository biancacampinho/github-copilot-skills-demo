using MicroDemo.Domain.Common;

namespace MicroDemo.Domain.Entities;

/// <summary>
/// Linha de um <see cref="Order"/>: liga um <see cref="Product"/> a um pedido, com a
/// quantidade e um <b>snapshot</b> do preço unitário no momento da compra (para que
/// alterações futuras do <see cref="Price"/> não afetem pedidos já realizados).
/// </summary>
public class OrderItem : BaseEntity
{
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int Quantity { get; set; }

    /// <summary>Snapshot do preço unitário no momento da compra.</summary>
    public decimal UnitPrice { get; set; }

    public string Currency { get; set; } = "EUR";

    /// <summary>Total da linha = <see cref="UnitPrice"/> × <see cref="Quantity"/>.</summary>
    public decimal LineTotal { get; set; }
}
