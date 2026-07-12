using MediatR;
using MicroDemo.Application.Common.Models;

namespace MicroDemo.Application.Features.Users.Commands.DeleteUser;

public record DeleteUserCommand(Guid Id) : IRequest<Result>;
