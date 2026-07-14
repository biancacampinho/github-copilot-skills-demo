using MediatR;
using SkillGhcDemo.Application.Common.Models;
using SkillGhcDemo.Application.Dtos;

namespace SkillGhcDemo.Application.Queries.Users;

/// <summary>Lists users, with an optional text filter (name/e-mail).</summary>
public record GetUsersQuery(string? Search = null, bool OnlyActive = false)
    : IRequest<Result<IReadOnlyList<UserDto>>>;
