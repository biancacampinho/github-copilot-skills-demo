using MediatR;
using SkillGhcDemo.Application.Commands.Users;
using SkillGhcDemo.Application.Common.Interfaces;
using SkillGhcDemo.Application.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace SkillGhcDemo.Application.Handlers.Commands.Users;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result>
{
    private readonly IAppDbContext _db;
    private readonly ILogger<DeleteUserCommandHandler> _logger;

    public DeleteUserCommandHandler(IAppDbContext db, ILogger<DeleteUserCommandHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _db.Users
            .Include(u => u.Orders)
            .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

        if (user is null)
            return Result.NotFound($"User {request.Id} not found.");

        if (user.Orders.Any())
            return Result.Failure(
                "Cannot delete a user with associated orders.",
                ResultErrorType.Conflict);

        _db.Users.Remove(user);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User {UserId} removed.", request.Id);
        return Result.Success();
    }
}
