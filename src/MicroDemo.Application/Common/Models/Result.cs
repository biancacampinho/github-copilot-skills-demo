namespace MicroDemo.Application.Common.Models;

/// <summary>
/// Standardized result envelope for all application-layer operations.
/// Avoids throwing exceptions for expected business flows (e.g. "not found").
/// </summary>
public class Result
{
    public bool Succeeded { get; init; }
    public string? Error { get; init; }
    public IReadOnlyList<string> ValidationErrors { get; init; } = Array.Empty<string>();

    /// <summary>Error category — used by the API to map the HTTP status.</summary>
    public ResultErrorType ErrorType { get; init; } = ResultErrorType.None;

    public static Result Success() => new() { Succeeded = true };

    public static Result Failure(string error, ResultErrorType type = ResultErrorType.Failure) =>
        new() { Succeeded = false, Error = error, ErrorType = type };

    public static Result NotFound(string error) =>
        new() { Succeeded = false, Error = error, ErrorType = ResultErrorType.NotFound };

    public static Result Invalid(IEnumerable<string> validationErrors) =>
        new()
        {
            Succeeded = false,
            Error = "One or more validation errors occurred.",
            ValidationErrors = validationErrors.ToArray(),
            ErrorType = ResultErrorType.Validation
        };
}

/// <summary>Typed version of <see cref="Result"/> that carries a payload.</summary>
public class Result<T> : Result
{
    public T? Data { get; init; }

    public static Result<T> Success(T data) => new() { Succeeded = true, Data = data };

    public static new Result<T> Failure(string error, ResultErrorType type = ResultErrorType.Failure) =>
        new() { Succeeded = false, Error = error, ErrorType = type };

    public static new Result<T> NotFound(string error) =>
        new() { Succeeded = false, Error = error, ErrorType = ResultErrorType.NotFound };

    public static new Result<T> Invalid(IEnumerable<string> validationErrors) =>
        new()
        {
            Succeeded = false,
            Error = "One or more validation errors occurred.",
            ValidationErrors = validationErrors.ToArray(),
            ErrorType = ResultErrorType.Validation
        };
}

public enum ResultErrorType
{
    None = 0,
    Failure = 1,
    Validation = 2,
    NotFound = 3,
    Conflict = 4
}
