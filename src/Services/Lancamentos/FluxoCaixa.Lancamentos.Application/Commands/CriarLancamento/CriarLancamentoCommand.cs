using MediatR;

namespace FluxoCaixa.Lancamentos.Application.Commands.CriarLancamento;

public record CriarLancamentoCommand(
    DateTime DataLancamento,
    int Tipo,
    decimal Valor,
    string Descricao,
    string? Observacao = null
) : IRequest<CriarLancamentoResponse>;