using MediatR;

namespace FluxoCaixa.Lancamentos.Application.Commands.AtualizarLancamento;

public record AtualizarLancamentoCommand(
    Guid Id,
    DateTime DataLancamento,
    int Tipo,
    decimal Valor,
    string Descricao,
    string? Observacao = null
) : IRequest<AtualizarLancamentoResponse>;