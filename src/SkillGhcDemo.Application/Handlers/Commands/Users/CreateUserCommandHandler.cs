using MediatR;
using SkillGhcDemo.Application.Commands.Users;
using SkillGhcDemo.Application.Common.Interfaces;
using SkillGhcDemo.Application.Common.Models;
using SkillGhcDemo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace SkillGhcDemo.Application.Handlers.Commands.Users;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<Guid>>
{
    private readonly IAppDbContext _db;
    private readonly ILogger<CreateUserCommandHandler> _logger;

    public CreateUserCommandHandler(IAppDbContext db, ILogger<CreateUserCommandHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var emailExists = await _db.Users
            .AnyAsync(u => u.Email == request.Email, cancellationToken);
        if (emailExists)
            return Result<Guid>.Failure($"A user with the e-mail '{request.Email}' already exists.", ResultErrorType.Conflict);

        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User {UserId} created.", user.Id);
        return Result<Guid>.Success(user.Id);
    }
}
