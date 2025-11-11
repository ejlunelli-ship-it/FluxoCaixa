using FluxoCaixa.Lancamentos.Application.DTOs;
using MediatR;

namespace FluxoCaixa.Lancamentos.Application.Queries.ObterLancamentosPorPeriodo;

public record ObterLancamentosPorPeriodoQuery(
    DateTime DataInicio,
    DateTime DataFim
) : IRequest<IEnumerable<LancamentoDto>>;