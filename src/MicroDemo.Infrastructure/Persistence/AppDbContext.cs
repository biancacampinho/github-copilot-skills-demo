using System.Reflection;
using MicroDemo.Application.Common.Interfaces;
using MicroDemo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MicroDemo.Infrastructure.Persistence;

public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Utente> Utenti => Set<Utente>();
    public DbSet<Price> Prices => Set<Price>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
