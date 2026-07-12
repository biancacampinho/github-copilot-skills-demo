using MediatR;
using MicroDemo.Application.Common.Interfaces;
using MicroDemo.Application.Common.Models;
using MicroDemo.Application.Features.Users.Dtos;
using Microsoft.EntityFrameworkCore;

namespace MicroDemo.Application.Features.Users.Queries.GetUserById;

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
            ? Result<UserDto>.NotFound($"User {request.Id} não encontrado.")
            : Result<UserDto>.Success(user.ToDto());
    }
}
