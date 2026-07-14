using FluentValidation.Results;

namespace SkillGhcDemo.Application.Common.Exceptions;

/// <summary>
/// Application validation exception. Wraps FluentValidation failures
/// in a simple format consumed by the API's exception-handling middleware.
/// </summary>
public class ValidationException : Exception
{
    public ValidationException()
        : base("One or more validation errors occurred.")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        Errors = failures
            .GroupBy(f => f.PropertyName, f => f.ErrorMessage)
            .ToDictionary(g => g.Key, g => g.ToArray());
    }

    public IDictionary<string, string[]> Errors { get; }

    /// <summary>Flattens all errors into a simple list of messages.</summary>
    public IEnumerable<string> FlattenedErrors =>
        Errors.SelectMany(kvp => kvp.Value.Select(msg => $"{kvp.Key}: {msg}"));
}
