using FluxoCaixa.Lancamentos.Application.DTOs;
using FluxoCaixa.Lancamentos.Domain.Repositories;
using MediatR;

namespace FluxoCaixa.Lancamentos.Application.Queries.ObterLancamentosPorPeriodo;

public class ObterLancamentosPorPeriodoQueryHandler
    : IRequestHandler<ObterLancamentosPorPeriodoQuery, IEnumerable<LancamentoDto>>
{
    private readonly ILancamentoRepository _repository;

    public ObterLancamentosPorPeriodoQueryHandler(ILancamentoRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<LancamentoDto>> Handle(
        ObterLancamentosPorPeriodoQuery request,
        CancellationToken cancellationToken)
    {
        var lancamentos = await _repository.ObterPorPeriodoAsync(
            request.DataInicio,
            request.DataFim,
            cancellationToken);

        return lancamentos.Select(l => new LancamentoDto(
            l.Id,
            l.DataLancamento,
            l.Tipo.ToString(),
            l.Valor,
            l.Descricao,
            l.Observacao,
            l.CriadoEm,
            l.AtualizadoEm
        ));
    }
}