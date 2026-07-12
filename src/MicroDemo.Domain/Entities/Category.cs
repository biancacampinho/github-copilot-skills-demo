using MicroDemo.Domain.Common;

namespace MicroDemo.Domain.Entities;

/// <summary>
/// Categoria de produtos (ex.: "Eletrónica", "Livros"). Um produto pertence a uma categoria.
/// </summary>
public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    // Navegação
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
