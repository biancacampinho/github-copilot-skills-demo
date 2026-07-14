using MediatR;
using MicroDemo.Application.Common.Models;
using MicroDemo.Application.Dtos;

namespace MicroDemo.Application.Queries.Users;

public record GetUserByIdQuery(Guid Id) : IRequest<Result<UserDto>>;
