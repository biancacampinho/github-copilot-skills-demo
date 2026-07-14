using FluentAssertions;
using MicroDemo.Application.Common.Models;

namespace MicroDemo.UnitTests.Common;

/// <summary>Assertion helpers for the <see cref="Result"/> envelope, shared across the handler and controller tests.</summary>
public static class ResultAssertions
{
    public static void ShouldBeSuccess(this Result result)
    {
        result.Succeeded.Should().BeTrue();
        result.Error.Should().BeNull();
    }

    public static void ShouldBeFailureOfType(this Result result, ResultErrorType type)
    {
        result.Succeeded.Should().BeFalse();
        result.ErrorType.Should().Be(type);
        result.Error.Should().NotBeNullOrWhiteSpace();
    }
}
