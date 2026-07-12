namespace MicroDemo.Domain.Enums;

/// <summary>Estado do ciclo de vida de um pedido (order) de e-commerce.</summary>
public enum OrderStatus
{
    Pending = 1,
    Paid = 2,
    Shipped = 3,
    Delivered = 4,
    Cancelled = 5
}
