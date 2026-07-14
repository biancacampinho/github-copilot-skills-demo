using MediatR;
using MicroDemo.Application.Common.Models;

namespace MicroDemo.Application.Commands.Users;

public record CreateUserCommand : IRequest<Result<Guid>>
{
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
}
