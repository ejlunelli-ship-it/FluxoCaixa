using FluxoCaixa.Lancamentos.Application.DTOs;
using MediatR;

namespace FluxoCaixa.Lancamentos.Application.Queries.ObterLancamentoPorId;

public record ObterLancamentoPorIdQuery(Guid Id) : IRequest<LancamentoDto?>;