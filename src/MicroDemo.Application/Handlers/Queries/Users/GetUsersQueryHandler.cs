using MediatR;
using MicroDemo.Application.Common.Interfaces;
using MicroDemo.Application.Common.Models;
using MicroDemo.Application.Dtos;
using MicroDemo.Application.Queries.Users;
using Microsoft.EntityFrameworkCore;

namespace MicroDemo.Application.Handlers.Queries.Users;

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
