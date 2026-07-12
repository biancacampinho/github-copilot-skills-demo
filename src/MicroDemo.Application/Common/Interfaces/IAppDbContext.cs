using MicroDemo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MicroDemo.Application.Common.Interfaces;

/// <summary>
/// Abstração do DbContext exposta à camada de aplicação. Permite testar handlers
/// sem depender diretamente da implementação concreta em Infrastructure.
/// </summary>
public interface IAppDbContext
{
    DbSet<Utente> Utenti { get; }
    DbSet<Price> Prices { get; }
    DbSet<Subscription> Subscriptions { get; }
    DbSet<Order> Orders { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
