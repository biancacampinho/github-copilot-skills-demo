namespace MicroDemo.Domain.Enums;

/// <summary>Estado de um pedido/cobrança.</summary>
public enum OrderStatus
{
    Pending = 1,
    Paid = 2,
    Failed = 3,
    Refunded = 4
}
