namespace FluxoCaixa.Lancamentos.Application.Commands.AtualizarLancamento;

public record AtualizarLancamentoResponse(
    Guid Id,
    DateTime DataLancamento,
    string Tipo,
    decimal Valor,
    string Descricao,
    DateTime AtualizadoEm
);