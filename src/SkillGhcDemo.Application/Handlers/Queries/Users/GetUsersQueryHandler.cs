using MediatR;
using SkillGhcDemo.Application.Common.Interfaces;
using SkillGhcDemo.Application.Common.Models;
using SkillGhcDemo.Application.Dtos;
using SkillGhcDemo.Application.Queries.Users;
using Microsoft.EntityFrameworkCore;

namespace SkillGhcDemo.Application.Handlers.Queries.Users;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, Result<IReadOnlyList<UserDto>>>
{
    private readonly IAppDbContext _db;

    public GetUsersQueryHandler(IAppDbContext db) => _db = db;

    public async Task<Result<IReadOnlyList<UserDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var query = _db.Users.AsNoTracking();

        if (request.OnlyActive)
            query = query.Where(u => u.IsActive);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            query = query.Where(u => u.FullName.Contains(term) || u.Email.Contains(term));
        }

        var users = await query
            .OrderBy(u => u.FullName)
            .ToListAsync(cancellationToken);

        IReadOnlyList<UserDto> dtos = users.Select(u => u.ToDto()).ToList();
        return Result<IReadOnlyList<UserDto>>.Success(dtos);
    }
}
