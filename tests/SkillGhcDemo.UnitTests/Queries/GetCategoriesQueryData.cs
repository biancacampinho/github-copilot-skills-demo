using SkillGhcDemo.Application.Queries.Categories;

namespace SkillGhcDemo.UnitTests.Queries;

/// <summary>Test-data variants of <see cref="GetCategoriesQuery"/> for handler tests.</summary>
public static class GetCategoriesQueryData
{
    public static GetCategoriesQuery All() => new(OnlyActive: false);

    public static GetCategoriesQuery OnlyActive() => new(OnlyActive: true);
}
