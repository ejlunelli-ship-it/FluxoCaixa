using FluxoCaixa.Lancamentos.Application.DTOs;
using MediatR;

namespace FluxoCaixa.Lancamentos.Application.Queries.ObterLancamentosPorPeriodo;

public record ObterLancamentosPorPeriodoQuery(
    DateOnly DataInicio,
    DateOnly DataFim
) : IRequest<IEnumerable<LancamentoDto>>;