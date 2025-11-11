using FluxoCaixa.Consolidado.Application.DTOs;
using FluxoCaixa.Consolidado.Domain.Repositories;
using MediatR;

namespace FluxoCaixa.Consolidado.Application.Queries.ObterConsolidadoPorData;

public class ObterConsolidadoPorDataQueryHandler
    : IRequestHandler<ObterConsolidadoPorDataQuery, ConsolidadoDiarioDto?>
{
    private readonly IConsolidadoDiarioRepository _repository;

    public ObterConsolidadoPorDataQueryHandler(IConsolidadoDiarioRepository repository)
    {
        _repository = repository;
    }

    public async Task<ConsolidadoDiarioDto?> Handle(
        ObterConsolidadoPorDataQuery request,
        CancellationToken cancellationToken)
    {
        var consolidado = await _repository.ObterPorDataAsync(
            request.Data,
            cancellationToken);

        if (consolidado == null)
            return null;

        return new ConsolidadoDiarioDto(
            consolidado.Id,
            consolidado.Data,
            consolidado.TotalCreditos,
            consolidado.TotalDebitos,
            consolidado.SaldoFinal,
            consolidado.QuantidadeLancamentos,
            consolidado.CriadoEm,
            consolidado.AtualizadoEm
        );
    }
}