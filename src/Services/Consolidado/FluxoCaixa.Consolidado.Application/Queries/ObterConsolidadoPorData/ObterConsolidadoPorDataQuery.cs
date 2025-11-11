using FluxoCaixa.Consolidado.Application.DTOs;
using MediatR;

namespace FluxoCaixa.Consolidado.Application.Queries.ObterConsolidadoPorData;

public record ObterConsolidadoPorDataQuery(DateOnly Data)
    : IRequest<ConsolidadoDiarioDto?>;