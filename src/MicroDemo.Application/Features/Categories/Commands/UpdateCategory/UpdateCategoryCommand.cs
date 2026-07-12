using MediatR;
using MicroDemo.Application.Common.Models;

namespace MicroDemo.Application.Features.Categories.Commands.UpdateCategory;

public record UpdateCategoryCommand : IRequest<Result>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsActive { get; init; } = true;
}
