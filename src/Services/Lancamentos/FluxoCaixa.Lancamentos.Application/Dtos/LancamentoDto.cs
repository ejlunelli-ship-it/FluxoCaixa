namespace FluxoCaixa.Lancamentos.Application.DTOs;

public record LancamentoDto(
    Guid Id,
    DateTime DataLancamento,
    string Tipo,
    decimal Valor,
    string Descricao,
    string? Observacao,
    DateTime CriadoEm,
    DateTime? AtualizadoEm
);