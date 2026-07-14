using MediatR;
using MicroDemo.Application.Common.Interfaces;
using MicroDemo.Application.Common.Models;
using MicroDemo.Application.Dtos;
using MicroDemo.Application.Queries.Users;
using Microsoft.EntityFrameworkCore;

namespace MicroDemo.Application.Handlers.Queries.Users;

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
