namespace FluxoCaixa.Consolidado.Application.DTOs;

public record ConsolidadoDiarioDto(
    Guid Id,
    DateOnly Data,
    decimal TotalCreditos,
    decimal TotalDebitos,
    decimal SaldoFinal,
    int QuantidadeLancamentos,
    DateTime CriadoEm,
    DateTime? AtualizadoEm
);