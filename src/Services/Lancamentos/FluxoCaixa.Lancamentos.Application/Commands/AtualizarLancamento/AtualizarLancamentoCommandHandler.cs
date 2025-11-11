using FluxoCaixa.Core.Domain;
using FluxoCaixa.Lancamentos.Domain.Enums;
using FluxoCaixa.Lancamentos.Domain.Repositories;
using MediatR;

namespace FluxoCaixa.Lancamentos.Application.Commands.AtualizarLancamento;

public class AtualizarLancamentoCommandHandler
    : IRequestHandler<AtualizarLancamentoCommand, AtualizarLancamentoResponse>
{
    private readonly ILancamentoRepository _repository;

    public AtualizarLancamentoCommandHandler(ILancamentoRepository repository)
    {
        _repository = repository;
    }

    public async Task<AtualizarLancamentoResponse> Handle(
        AtualizarLancamentoCommand request,
        CancellationToken cancellationToken)
    {
        var lancamento = await _repository.ObterPorIdAsync(request.Id, cancellationToken);

        if (lancamento == null)
            throw new DomainException($"Lançamento com Id {request.Id} não encontrado");

        lancamento.Atualizar(
            request.DataLancamento,
            (TipoLancamento)request.Tipo,
            request.Valor,
            request.Descricao,
            request.Observacao
        );

        _repository.Atualizar(lancamento);
        await _repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return new AtualizarLancamentoResponse(
            lancamento.Id,
            lancamento.DataLancamento,
            lancamento.Tipo.ToString(),
            lancamento.Valor,
            lancamento.Descricao,
            lancamento.AtualizadoEm ?? lancamento.CriadoEm
        );
    }
}