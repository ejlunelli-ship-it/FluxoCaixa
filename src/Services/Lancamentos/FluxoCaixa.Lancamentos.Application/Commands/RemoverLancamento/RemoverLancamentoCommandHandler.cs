using FluxoCaixa.Core.Domain;
using FluxoCaixa.Lancamentos.Domain.Repositories;
using MediatR;

namespace FluxoCaixa.Lancamentos.Application.Commands.RemoverLancamento;

public class RemoverLancamentoCommandHandler
    : IRequestHandler<RemoverLancamentoCommand, RemoverLancamentoResponse>
{
    private readonly ILancamentoRepository _repository;

    public RemoverLancamentoCommandHandler(ILancamentoRepository repository)
    {
        _repository = repository;
    }

    public async Task<RemoverLancamentoResponse> Handle(
        RemoverLancamentoCommand request,
        CancellationToken cancellationToken)
    {
        var lancamento = await _repository.ObterPorIdAsync(request.Id, cancellationToken);

        if (lancamento == null)
            throw new DomainException($"Lançamento com Id {request.Id} não encontrado");

        _repository.Remover(lancamento);
        await _repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return new RemoverLancamentoResponse(
            true,
            $"Lançamento {request.Id} removido com sucesso"
        );
    }
}