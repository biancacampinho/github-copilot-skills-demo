using MediatR;
using SkillGhcDemo.Application.Common.Interfaces;
using SkillGhcDemo.Application.Common.Models;
using SkillGhcDemo.Application.Dtos;
using SkillGhcDemo.Application.Queries.Users;
using Microsoft.EntityFrameworkCore;

namespace SkillGhcDemo.Application.Handlers.Queries.Users;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
{
    private readonly IAppDbContext _db;

    public GetUserByIdQueryHandler(IAppDbContext db) => _db = db;

    public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

        return user is null
            ? Result<UserDto>.NotFound($"User {request.Id} not found.")
            : Result<UserDto>.Success(user.ToDto());
    }
}
