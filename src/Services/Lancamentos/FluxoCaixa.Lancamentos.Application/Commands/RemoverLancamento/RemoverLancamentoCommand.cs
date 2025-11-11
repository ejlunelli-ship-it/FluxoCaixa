using MediatR;

namespace FluxoCaixa.Lancamentos.Application.Commands.RemoverLancamento;

public record RemoverLancamentoCommand(Guid Id) : IRequest<RemoverLancamentoResponse>;