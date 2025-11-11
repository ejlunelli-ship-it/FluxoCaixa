using FluxoCaixa.Consolidado.Application.DTOs;
using MediatR;

namespace FluxoCaixa.Consolidado.Application.Queries.ObterConsolidadoPorPeriodo;

public record ObterConsolidadoPorPeriodoQuery(
    DateOnly DataInicio,
    DateOnly DataFim
) : IRequest<IEnumerable<ConsolidadoDiarioDto>>;