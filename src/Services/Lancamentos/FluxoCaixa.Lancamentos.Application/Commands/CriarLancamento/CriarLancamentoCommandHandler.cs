using FluxoCaixa.Lancamentos.Domain.Entities;
using FluxoCaixa.Lancamentos.Domain.Enums;
using FluxoCaixa.Lancamentos.Domain.Repositories;
using MediatR;

namespace FluxoCaixa.Lancamentos.Application.Commands.CriarLancamento;

public class CriarLancamentoCommandHandler
    : IRequestHandler<CriarLancamentoCommand, CriarLancamentoResponse>
{
    private readonly ILancamentoRepository _repository;

    public CriarLancamentoCommandHandler(ILancamentoRepository repository)
    {
        _repository = repository;
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

        return new CriarLancamentoResponse(
            lancamento.Id,
            lancamento.DataLancamento,
            lancamento.Tipo.ToString(),
            lancamento.Valor,
            lancamento.Descricao
        );
    }
}