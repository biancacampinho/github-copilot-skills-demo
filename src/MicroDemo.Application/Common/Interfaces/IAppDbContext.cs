using MicroDemo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MicroDemo.Application.Common.Interfaces;

/// <summary>
/// DbContext abstraction exposed to the application layer. Allows handlers to be
/// tested without depending directly on the concrete implementation in Infrastructure.
/// </summary>
public interface IAppDbContext
{
    DbSet<User> Users { get; }
    DbSet<Category> Categories { get; }
    DbSet<Product> Products { get; }
    DbSet<Price> Prices { get; }
    DbSet<Order> Orders { get; }
    DbSet<OrderItem> OrderItems { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
