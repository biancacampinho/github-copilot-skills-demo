using MediatR;
using MicroDemo.Application.Common.Interfaces;
using MicroDemo.Application.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MicroDemo.Application.Features.Users.Commands.DeleteUser;

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
            return Result.NotFound($"User {request.Id} não encontrado.");

        if (user.Orders.Any())
            return Result.Failure(
                "Não é possível excluir um utilizador com pedidos associados.",
                ResultErrorType.Conflict);

        _db.Users.Remove(user);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User {UserId} removido.", request.Id);
        return Result.Success();
    }
}
