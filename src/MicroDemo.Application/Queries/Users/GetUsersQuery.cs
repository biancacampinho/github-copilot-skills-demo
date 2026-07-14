using MediatR;
using MicroDemo.Application.Common.Models;
using MicroDemo.Application.Dtos;

namespace MicroDemo.Application.Queries.Users;

/// <summary>Lists users, with an optional text filter (name/e-mail).</summary>
public record GetUsersQuery(string? Search = null, bool OnlyActive = false)
    : IRequest<Result<IReadOnlyList<UserDto>>>;
