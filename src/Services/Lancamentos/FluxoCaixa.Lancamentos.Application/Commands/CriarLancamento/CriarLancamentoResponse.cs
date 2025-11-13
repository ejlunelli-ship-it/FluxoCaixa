namespace FluxoCaixa.Lancamentos.Application.Commands.CriarLancamento;

public record CriarLancamentoResponse(
    Guid Id,
    DateTime DataLancamento,
    string Tipo,
    decimal Valor,
    string Descricao,
    string Observacao
);