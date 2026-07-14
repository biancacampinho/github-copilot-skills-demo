using SkillGhcDemo.Application.Queries.Users;

namespace SkillGhcDemo.UnitTests.Queries;

/// <summary>Test-data variants of <see cref="GetUsersQuery"/> for handler tests.</summary>
public static class GetUsersQueryData
{
    public static GetUsersQuery All() => new(Search: null, OnlyActive: false);

    public static GetUsersQuery OnlyActive() => new(Search: null, OnlyActive: true);

    public static GetUsersQuery Search(string term) => new(Search: term, OnlyActive: false);
}
