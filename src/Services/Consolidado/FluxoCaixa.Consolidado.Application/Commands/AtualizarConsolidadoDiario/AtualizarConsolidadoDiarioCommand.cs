using MediatR;

namespace FluxoCaixa.Consolidado.Application.Commands.AtualizarConsolidadoDiario;

public record AtualizarConsolidadoDiarioCommand(
    DateOnly Data,
    decimal TotalCreditos,
    decimal TotalDebitos,
    int QuantidadeLancamentos
) : IRequest<AtualizarConsolidadoDiarioResponse>;