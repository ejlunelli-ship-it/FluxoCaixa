using FluxoCaixa.Lancamentos.Domain.Entities;
using FluxoCaixa.Lancamentos.Domain.Enums;
using FluxoCaixa.Lancamentos.Domain.Repositories;
using FluxoCaixa.EventBus.Events;
using MassTransit;
using MediatR;

namespace FluxoCaixa.Lancamentos.Application.Commands.CriarLancamento;

public class CriarLancamentoCommandHandler
    : IRequestHandler<CriarLancamentoCommand, CriarLancamentoResponse>
{
    private readonly ILancamentoRepository _repository;
    private readonly IPublishEndpoint _publishEndpoint; 

    public CriarLancamentoCommandHandler(
        ILancamentoRepository repository,
        IPublishEndpoint publishEndpoint)
    {
        _repository = repository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<CriarLancamentoResponse> Handle(
        CriarLancamentoCommand request,
        CancellationToken cancellationToken)
    {
        var lancamento = new Lancamento(
            request.DataLancamento,
            (TipoLancamento)request.Tipo,
            request.Valor,
            request.Descricao,
            request.Observacao
        );

        await _repository.AdicionarAsync(lancamento, cancellationToken);
        await _repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        var evento = new LancamentoCriadoEvent
        {
            LancamentoId = lancamento.Id,
            Data = DateOnly.FromDateTime(lancamento.DataLancamento),
            Tipo = (int)lancamento.Tipo,
            Valor = lancamento.Valor,
            CriadoEm = lancamento.CriadoEm
        };

        await _publishEndpoint.Publish(evento, cancellationToken);

        return new CriarLancamentoResponse(
            lancamento.Id,
            lancamento.DataLancamento,
            lancamento.Tipo.ToString(),
            lancamento.Valor,
            lancamento.Descricao,
            lancamento.Observacao
        );
    }
}