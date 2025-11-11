using FluxoCaixa.Consolidado.Domain.Entities;
using FluxoCaixa.Consolidado.Domain.Repositories;
using MediatR;

namespace FluxoCaixa.Consolidado.Application.Commands.AtualizarConsolidadoDiario;

public class AtualizarConsolidadoDiarioCommandHandler
    : IRequestHandler<AtualizarConsolidadoDiarioCommand, AtualizarConsolidadoDiarioResponse>
{
    private readonly IConsolidadoDiarioRepository _repository;

    public AtualizarConsolidadoDiarioCommandHandler(IConsolidadoDiarioRepository repository)
    {
        _repository = repository;
    }

    public async Task<AtualizarConsolidadoDiarioResponse> Handle(
        AtualizarConsolidadoDiarioCommand request,
        CancellationToken cancellationToken)
    {
        var consolidadoExistente = await _repository.ObterPorDataAsync(
            request.Data,
            cancellationToken);

        bool foiCriado = false;
        ConsolidadoDiario consolidado;

        if (consolidadoExistente == null)
        {
            consolidado = new ConsolidadoDiario(request.Data);
            consolidado.AtualizarSaldo(
                request.TotalCreditos,
                request.TotalDebitos,
                request.QuantidadeLancamentos);

            await _repository.AdicionarAsync(consolidado, cancellationToken);
            foiCriado = true;
        }
        else
        {
            consolidado = consolidadoExistente;
            consolidado.AtualizarSaldo(
                request.TotalCreditos,
                request.TotalDebitos,
                request.QuantidadeLancamentos);

            _repository.Atualizar(consolidado);
        }

        await _repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return new AtualizarConsolidadoDiarioResponse(
            consolidado.Id,
            consolidado.Data,
            consolidado.SaldoFinal,
            foiCriado
        );
    }
}