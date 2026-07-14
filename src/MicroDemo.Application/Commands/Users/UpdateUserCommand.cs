using MediatR;
using MicroDemo.Application.Common.Models;

namespace MicroDemo.Application.Commands.Users;

public record UpdateUserCommand : IRequest<Result>
{
    public Guid Id { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public bool IsActive { get; init; } = true;
}
