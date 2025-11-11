using FluxoCaixa.Consolidado.Application.Commands.AtualizarConsolidadoDiario;
using FluxoCaixa.Consolidado.Domain.Repositories;
using FluxoCaixa.EventBus.Events;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FluxoCaixa.Consolidado.Infrastructure.Consumers;

public class LancamentoCriadoConsumer : IConsumer<LancamentoCriadoEvent>
{
    private readonly IMediator _mediator;
    private readonly IConsolidadoDiarioRepository _repository;
    private readonly ILogger<LancamentoCriadoConsumer> _logger;

    public LancamentoCriadoConsumer(
    IMediator mediator, IConsolidadoDiarioRepository repository, ILogger<LancamentoCriadoConsumer> logger)
    {
        _mediator = mediator;
        _repository = repository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<LancamentoCriadoEvent> context)
    {
        var evento = context.Message;

        _logger.LogInformation("Evento recebido: LancamentoCriado - ID: {LancamentoId}, Data: {Data}, Tipo: {Tipo}, Valor: {Valor}", evento.LancamentoId, evento.Data, evento.Tipo, evento.Valor);

        try
        {
            var consolidadoExistente = await _repository.ObterPorDataAsync(
            evento.Data, context.CancellationToken);

            decimal totalCreditos = consolidadoExistente?.TotalCreditos ?? 0;
            decimal totalDebitos = consolidadoExistente?.TotalDebitos ?? 0;
            int quantidadeLancamentos = consolidadoExistente?.QuantidadeLancamentos ?? 0;

            if (evento.Tipo == 1) // Crédito
            {
                totalCreditos += evento.Valor;
            }
            else if (evento.Tipo == 2) // Débito
            {
                totalDebitos += evento.Valor;
            }

            quantidadeLancamentos++;

            var command = new AtualizarConsolidadoDiarioCommand(
            evento.Data, totalCreditos, totalDebitos, quantidadeLancamentos);

            var result = await _mediator.Send(command, context.CancellationToken);

            _logger.LogInformation("Consolidado atualizado com sucesso - Data: {Data}, Saldo: {Saldo}", evento.Data, result.SaldoFinal);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar evento LancamentoCriado: {LancamentoId}", evento.LancamentoId);

            throw;
        }
    }
}