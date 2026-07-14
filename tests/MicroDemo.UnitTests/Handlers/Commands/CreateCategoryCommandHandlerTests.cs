using FluentAssertions;
using MicroDemo.Application.Common.Models;
using MicroDemo.Application.Handlers.Commands.Categories;
using MicroDemo.Domain.Entities;
using MicroDemo.UnitTests.Commands;
using MicroDemo.UnitTests.Common;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MicroDemo.UnitTests.Handlers.Commands;

public class CreateCategoryCommandHandlerTests
{
    [Fact]
    public async Task Succeeds_for_valid_request()
    {
        using var db = TestDbContextFactory.Create();
        var handler = new CreateCategoryCommandHandler(db, TestLogger.For<CreateCategoryCommandHandler>());

        var result = await handler.Handle(CreateCategoryCommandData.Valid(), CancellationToken.None);

        result.ShouldBeSuccess();
        (await db.Categories.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task Returns_conflict_for_duplicate_name()
    {
        using var db = TestDbContextFactory.Create();
        db.Categories.Add(new Category { Name = "Electronics" });
        await db.SaveChangesAsync();

        var handler = new CreateCategoryCommandHandler(db, TestLogger.For<CreateCategoryCommandHandler>());
        var result = await handler.Handle(CreateCategoryCommandData.WithName("Electronics"), CancellationToken.None);

        result.ShouldBeFailureOfType(ResultErrorType.Conflict);
    }
}
