namespace MicroDemo.Application.Common.Models;

/// <summary>
/// Envelope de resultado padronizado para todas as operações da camada de aplicação.
/// Evita lançar exceções para fluxos de negócio esperados (ex.: "não encontrado").
/// </summary>
public class Result
{
    public bool Succeeded { get; init; }
    public string? Error { get; init; }
    public IReadOnlyList<string> ValidationErrors { get; init; } = Array.Empty<string>();

    /// <summary>Categoria do erro — usada pela API para mapear o status HTTP.</summary>
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
            Error = "Um ou mais erros de validação ocorreram.",
            ValidationErrors = validationErrors.ToArray(),
            ErrorType = ResultErrorType.Validation
        };
}

/// <summary>Versão tipada de <see cref="Result"/> que carrega um payload.</summary>
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
            Error = "Um ou mais erros de validação ocorreram.",
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
