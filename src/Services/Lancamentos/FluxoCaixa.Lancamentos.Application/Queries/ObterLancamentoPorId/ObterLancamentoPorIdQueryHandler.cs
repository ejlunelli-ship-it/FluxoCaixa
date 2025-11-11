using FluxoCaixa.Lancamentos.Application.DTOs;
using FluxoCaixa.Lancamentos.Domain.Repositories;
using MediatR;

namespace FluxoCaixa.Lancamentos.Application.Queries.ObterLancamentoPorId;

public class ObterLancamentoPorIdQueryHandler
    : IRequestHandler<ObterLancamentoPorIdQuery, LancamentoDto?>
{
    private readonly ILancamentoRepository _repository;

    public ObterLancamentoPorIdQueryHandler(ILancamentoRepository repository)
    {
        _repository = repository;
    }

    public async Task<LancamentoDto?> Handle(
        ObterLancamentoPorIdQuery request,
        CancellationToken cancellationToken)
    {
        var lancamento = await _repository.ObterPorIdAsync(request.Id, cancellationToken);

        if (lancamento == null)
            return null;

        return new LancamentoDto(
            lancamento.Id,
            lancamento.DataLancamento,
            lancamento.Tipo.ToString(),
            lancamento.Valor,
            lancamento.Descricao,
            lancamento.Observacao,
            lancamento.CriadoEm,
            lancamento.AtualizadoEm
        );
    }
}