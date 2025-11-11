namespace FluxoCaixa.Consolidado.Application.Commands.AtualizarConsolidadoDiario;

public record AtualizarConsolidadoDiarioResponse(
    Guid Id,
    DateOnly Data,
    decimal SaldoFinal,
    bool FoiCriado
);