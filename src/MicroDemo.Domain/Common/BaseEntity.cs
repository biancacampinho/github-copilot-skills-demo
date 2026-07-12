namespace MicroDemo.Domain.Common;

/// <summary>
/// Base para todas as entidades do domínio. Fornece identidade e campos de auditoria.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAtUtc { get; set; }
}
