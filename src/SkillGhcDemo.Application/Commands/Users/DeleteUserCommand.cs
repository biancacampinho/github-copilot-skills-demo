using MediatR;
using SkillGhcDemo.Application.Common.Models;

namespace SkillGhcDemo.Application.Commands.Users;

public record DeleteUserCommand(Guid Id) : IRequest<Result>;
