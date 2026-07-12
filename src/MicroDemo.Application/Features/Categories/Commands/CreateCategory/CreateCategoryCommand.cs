using MediatR;
using MicroDemo.Application.Common.Models;

namespace MicroDemo.Application.Features.Categories.Commands.CreateCategory;

public record CreateCategoryCommand : IRequest<Result<Guid>>
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsActive { get; init; } = true;
}
