using FluxoCaixa.Consolidado.Application.DTOs;
using FluxoCaixa.Consolidado.Domain.Repositories;
using MediatR;

namespace FluxoCaixa.Consolidado.Application.Queries.ObterConsolidadoPorPeriodo;

public class ObterConsolidadoPorPeriodoQueryHandler
    : IRequestHandler<ObterConsolidadoPorPeriodoQuery, IEnumerable<ConsolidadoDiarioDto>>
{
    private readonly IConsolidadoDiarioRepository _repository;

    public ObterConsolidadoPorPeriodoQueryHandler(IConsolidadoDiarioRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ConsolidadoDiarioDto>> Handle(
        ObterConsolidadoPorPeriodoQuery request,
        CancellationToken cancellationToken)
    {
        var consolidados = await _repository.ObterPorPeriodoAsync(
            request.DataInicio,
            request.DataFim,
            cancellationToken);

        return consolidados.Select(c => new ConsolidadoDiarioDto(
            c.Id,
            c.Data,
            c.TotalCreditos,
            c.TotalDebitos,
            c.SaldoFinal,
            c.QuantidadeLancamentos,
            c.CriadoEm,
            c.AtualizadoEm
        ));
    }
}