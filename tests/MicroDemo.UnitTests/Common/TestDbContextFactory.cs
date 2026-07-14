using MicroDemo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MicroDemo.UnitTests.Common;

/// <summary>
/// Factory for <see cref="AppDbContext"/> using the EF Core InMemory provider.
/// Each context gets a unique database name to guarantee isolation between tests.
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
