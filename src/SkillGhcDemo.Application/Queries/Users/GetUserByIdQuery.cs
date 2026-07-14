using MediatR;
using SkillGhcDemo.Application.Common.Models;
using SkillGhcDemo.Application.Dtos;

namespace SkillGhcDemo.Application.Queries.Users;

public record GetUserByIdQuery(Guid Id) : IRequest<Result<UserDto>>;
