using MicroDemo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MicroDemo.UnitTests.Common;

/// <summary>
/// Fábrica de <see cref="AppDbContext"/> usando o provider InMemory do EF Core.
/// Cada contexto recebe um nome de base único para garantir isolamento entre testes.
/// </summary>
public static class TestDbContextFactory
{
    public static AppDbContext Create()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"MicroDemoTests_{Guid.NewGuid()}")
            .EnableSensitiveDataLogging()
            .Options;

        return new AppDbContext(options);
    }
}
