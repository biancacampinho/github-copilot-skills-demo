using MediatR;
using MicroDemo.Application.Common.Models;
using MicroDemo.Application.Features.Users.Dtos;

namespace MicroDemo.Application.Features.Users.Queries.GetUsers;

/// <summary>Lista utilizadores, com filtro opcional de texto (nome/e-mail).</summary>
public record GetUsersQuery(string? Search = null, bool OnlyActive = false)
    : IRequest<Result<IReadOnlyList<UserDto>>>;
