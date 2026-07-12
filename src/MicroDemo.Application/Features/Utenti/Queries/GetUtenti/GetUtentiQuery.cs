using MediatR;
using MicroDemo.Application.Common.Models;
using MicroDemo.Application.Features.Utenti.Dtos;

namespace MicroDemo.Application.Features.Utenti.Queries.GetUtenti;

/// <summary>Lista utenti, com filtro opcional de texto (nome/e-mail).</summary>
public record GetUtentiQuery(string? Search = null, bool OnlyActive = false)
    : IRequest<Result<IReadOnlyList<UtenteDto>>>;
