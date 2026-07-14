using Microsoft.Extensions.Logging;
using Moq;

namespace MicroDemo.UnitTests.Common;

/// <summary>Provides no-op <see cref="ILogger{T}"/> instances for handlers under test.</summary>
public static class TestLogger
{
    public static ILogger<T> For<T>() => new Mock<ILogger<T>>().Object;
}
