using MediatR;
using MicroDemo.Application.Common.Models;

namespace MicroDemo.Application.Commands.Users;

public record DeleteUserCommand(Guid Id) : IRequest<Result>;
