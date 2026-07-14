using MediatR;
using SkillGhcDemo.Application.Common.Models;

namespace SkillGhcDemo.Application.Commands.Products;

public record CreateProductCommand : IRequest<Result<Guid>>
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Sku { get; init; } = string.Empty;
    public Guid CategoryId { get; init; }
    public bool IsActive { get; init; } = true;
}
