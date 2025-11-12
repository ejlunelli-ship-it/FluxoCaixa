using FluxoCaixa.Consolidado.Application.Commands.AtualizarConsolidadoDiario;
using FluxoCaixa.Consolidado.Domain.Entities;
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
    private const int MaxRetries = 3;

    public LancamentoCriadoConsumer(
        IMediator mediator,
        IConsolidadoDiarioRepository repository,
        ILogger<LancamentoCriadoConsumer> logger)
    {
        _mediator = mediator;
        _repository = repository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<LancamentoCriadoEvent> context)
    {
        var evento = context.Message;

        _logger.LogInformation(
            "Evento recebido: LancamentoCriado - ID: {LancamentoId}, Data: {Data}, Tipo: {Tipo}, Valor: {Valor}",
            evento.LancamentoId, evento.Data, evento.Tipo, evento.Valor);

        var retryCount = 0;
        var processed = false;

        while (!processed && retryCount < MaxRetries)
        {
            try
            {
                var consolidadoExistente = await _repository.ObterPorDataAsync(
                    evento.Data, context.CancellationToken);

                if (consolidadoExistente == null)
                {
                    consolidadoExistente = new ConsolidadoDiario(evento.Data);
                    await _repository.AdicionarAsync(consolidadoExistente, context.CancellationToken);
                }

                if (evento.Tipo == 1) 
                {
                    consolidadoExistente.AdicionarCredito(evento.Valor);
                }
                else if (evento.Tipo == 2)
                {
                    consolidadoExistente.AdicionarDebito(evento.Valor);
                }

                _repository.Atualizar(consolidadoExistente);

                await _repository.UnitOfWork.SaveChangesAsync(context.CancellationToken);

                processed = true;

                _logger.LogInformation(
                    "Consolidado atualizado com sucesso - Data: {Data}, Saldo: {Saldo}",
                    evento.Data, consolidadoExistente.SaldoFinal);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
            {
                retryCount++;
                _logger.LogWarning(
                    "Conflito de concorrência detectado. Tentativa {Retry} de {MaxRetries}",
                    retryCount, MaxRetries);

                if (retryCount >= MaxRetries)
                {
                    _logger.LogError(
                        "Falha ao processar evento após {MaxRetries} tentativas: {LancamentoId}",
                        MaxRetries, evento.LancamentoId);
                    throw;
                }

                await Task.Delay(TimeSpan.FromMilliseconds(100 * retryCount), context.CancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar evento LancamentoCriado: {LancamentoId}", evento.LancamentoId);
                throw;
            }
        }
    }
}