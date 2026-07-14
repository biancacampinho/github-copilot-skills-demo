using SkillGhcDemo.Application.Queries.Users;

namespace SkillGhcDemo.UnitTests.Queries;

/// <summary>Test-data variants of <see cref="GetUserByIdQuery"/> for handler tests.</summary>
public static class GetUserByIdQueryData
{
    public static GetUserByIdQuery ForId(Guid id) => new(id);

    public static GetUserByIdQuery Unknown() => new(Guid.NewGuid());
}
