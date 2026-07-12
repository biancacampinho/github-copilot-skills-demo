using FluentValidation.Results;

namespace MicroDemo.Application.Common.Exceptions;

/// <summary>
/// Exceção de validação da aplicação. Encapsula as falhas do FluentValidation
/// num formato simples consumido pelo middleware de tratamento de exceções da API.
/// </summary>
public class ValidationException : Exception
{
    public ValidationException()
        : base("Um ou mais erros de validação ocorreram.")
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

    /// <summary>Achata todos os erros numa lista simples de mensagens.</summary>
    public IEnumerable<string> FlattenedErrors =>
        Errors.SelectMany(kvp => kvp.Value.Select(msg => $"{kvp.Key}: {msg}"));
}
