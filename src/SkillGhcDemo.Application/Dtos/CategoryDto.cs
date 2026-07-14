using SkillGhcDemo.Domain.Entities;

namespace SkillGhcDemo.Application.Dtos;

/// <summary>Read projection of <see cref="Category"/>.</summary>
public class CategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}

public static class CategoryMappingExtensions
{
    public static CategoryDto ToDto(this Category category) => new()
    {
        Id = category.Id,
        Name = category.Name,
        Description = category.Description,
        IsActive = category.IsActive,
        CreatedAtUtc = category.CreatedAtUtc
    };
}
