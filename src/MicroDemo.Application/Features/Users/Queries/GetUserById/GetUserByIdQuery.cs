using MediatR;
using MicroDemo.Application.Common.Models;
using MicroDemo.Application.Features.Users.Dtos;

namespace MicroDemo.Application.Features.Users.Queries.GetUserById;

public record GetUserByIdQuery(Guid Id) : IRequest<Result<UserDto>>;
