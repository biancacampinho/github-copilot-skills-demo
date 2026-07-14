using MediatR;
using SkillGhcDemo.Application.Common.Models;

namespace SkillGhcDemo.Application.Commands.Categories;

public record CreateCategoryCommand : IRequest<Result<Guid>>
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsActive { get; init; } = true;
}
