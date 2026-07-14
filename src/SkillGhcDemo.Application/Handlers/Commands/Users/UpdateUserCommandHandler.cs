using MediatR;
using SkillGhcDemo.Application.Commands.Users;
using SkillGhcDemo.Application.Common.Interfaces;
using SkillGhcDemo.Application.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace SkillGhcDemo.Application.Handlers.Commands.Users;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result>
{
    private readonly IAppDbContext _db;
    private readonly ILogger<UpdateUserCommandHandler> _logger;

    public UpdateUserCommandHandler(IAppDbContext db, ILogger<UpdateUserCommandHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);
        if (user is null)
            return Result.NotFound($"User {request.Id} not found.");

        var emailTaken = await _db.Users
            .AnyAsync(u => u.Email == request.Email && u.Id != request.Id, cancellationToken);
        if (emailTaken)
            return Result.Failure($"The e-mail '{request.Email}' is already in use by another user.", ResultErrorType.Conflict);

        user.FullName = request.FullName;
        user.Email = request.Email;
        user.PhoneNumber = request.PhoneNumber;
        user.IsActive = request.IsActive;
        user.UpdatedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User {UserId} updated.", user.Id);
        return Result.Success();
    }
}
