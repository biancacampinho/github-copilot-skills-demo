using FluentAssertions;
using SkillGhcDemo.Application.Handlers.Commands.Orders;
using SkillGhcDemo.UnitTests.Commands;
using SkillGhcDemo.UnitTests.Common;
using Xunit;

namespace SkillGhcDemo.UnitTests.Handlers.Commands;

public class CreateOrderCommandHandlerTests
{
    /// <summary>
    /// This endpoint is reserved for manual implementation (via custom skill); the handler
    /// is currently a stub. This test documents the current behavior: calling Handle throws
    /// <see cref="NotImplementedException"/>. Replace it with success/business-rule assertions
    /// once the handler is implemented.
    /// </summary>
    [Fact]
    public async Task Handle_throws_notimplemented_while_stub()
    {
        using var db = TestDbContextFactory.Create();
        var handler = new CreateOrderCommandHandler(db, TestLogger.For<CreateOrderCommandHandler>());

        await FluentActions
            .Awaiting(() => handler.Handle(CreateOrderCommandData.Valid(), CancellationToken.None))
            .Should().ThrowAsync<NotImplementedException>();
    }
}
