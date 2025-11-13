using FluxoCaixa.Consolidado.Application.DTOs;
using FluxoCaixa.Consolidado.Application.Queries.ObterConsolidadoPorData;
using FluxoCaixa.Consolidado.Application.Queries.ObterConsolidadoPorPeriodo;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FluxoCaixa.Consolidado.API.Controllers;

/// <summary>
/// Controller para consultar consolidados diários
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class ConsolidadoController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ConsolidadoController> _logger;

    public ConsolidadoController(IMediator mediator, ILogger<ConsolidadoController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Obtém o consolidado diário de uma data específica
    /// </summary>
    /// <param name="data">Data no formato YYYY-MM-DD</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Consolidado diário</returns>
    /// <response code="200">Consolidado encontrado</response>
    /// <response code="404">Consolidado não encontrado para a data</response>
    /// <response code="401">Não autenticado</response>
    [HttpGet("diario/{data}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize(Policy = "Viewer")] 
    public async Task<ActionResult> ObterPorData(
        DateOnly data,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Buscando consolidado para data: {Data}", data);

        var query = new ObterConsolidadoPorDataQuery(data);
        var result = await _mediator.Send(query, cancellationToken);

        if (result == null)
        {
            _logger.LogWarning("Consolidado não encontrado para data: {Data}", data);
            return NotFound(new { message = $"Consolidado não encontrado para a data {data:yyyy-MM-dd}" });
        }

        return Ok(result);
    }

    /// <summary>
    /// Obtém consolidados de um período
    /// </summary>
    /// <param name="dataInicio">Data inicial no formato YYYY-MM-DD</param>
    /// <param name="dataFim">Data final no formato YYYY-MM-DD</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de consolidados</returns>
    /// <response code="200">Lista de consolidados</response>
    /// <response code="401">Não autenticado</response>
    [HttpGet("periodo")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize(Policy = "Viewer")] 
    public async Task<ActionResult> ObterPorPeriodo(
        [FromQuery][Required] DateOnly dataInicio,
        [FromQuery][Required] DateOnly dataFim,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Buscando consolidados entre {DataInicio} e {DataFim}", dataInicio, dataFim);

        if (dataInicio > dataFim)
        {
            return BadRequest(new { message = "Data inicial não pode ser maior que data final" });
        }

        var query = new ObterConsolidadoPorPeriodoQuery(dataInicio, dataFim);
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(new
        {
            dataInicio,
            dataFim,
            quantidade = result.Count(),
            consolidados = result
        });
    }

    /// <summary>
    /// Obtém estatísticas do consolidado no período
    /// </summary>
    /// <param name="dataInicio">Data inicial</param>
    /// <param name="dataFim">Data final</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Estatísticas consolidadas</returns>
    /// <response code="200">Estatísticas encontradas</response>
    /// <response code="401">Não autenticado</response>
    [HttpGet("periodo/estatisticas")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize(Policy = "Viewer")]
    public async Task<ActionResult> ObterEstatisticas(
        [FromQuery][Required] DateOnly dataInicio,
        [FromQuery][Required] DateOnly dataFim,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Calculando estatísticas entre {DataInicio} e {DataFim}", dataInicio, dataFim);

        if (dataInicio > dataFim)
        {
            return BadRequest(new { message = "Data inicial não pode ser maior que data final" });
        }

        var query = new ObterConsolidadoPorPeriodoQuery(dataInicio, dataFim);
        var consolidados = await _mediator.Send(query, cancellationToken);

        var consolidadosDict = consolidados.ToDictionary(c => c.Data);

        var todasAsDatas = new List<ConsolidadoDiarioDto>();
        for (var data = dataInicio; data <= dataFim; data = data.AddDays(1))
        {
            if (consolidadosDict.TryGetValue(data, out var consolidado))
            {
                todasAsDatas.Add(consolidado);
            }
            else
            {
                todasAsDatas.Add(new ConsolidadoDiarioDto(
                    Guid.Empty,
                    data,
                    TotalCreditos: 0,
                    TotalDebitos: 0,
                    SaldoFinal: 0,
                    QuantidadeLancamentos: 0,
                    CriadoEm: DateTime.UtcNow,
                    AtualizadoEm: null
                ));
            }
        }

        var totalDias = todasAsDatas.Count;
        var diasComMovimentacao = consolidadosDict.Count;

        var estatisticas = new
        {
            periodo = new { dataInicio, dataFim },
            totalDias,
            diasComMovimentacao,
            diasSemMovimentacao = totalDias - diasComMovimentacao,
            totalCreditos = todasAsDatas.Sum(c => c.TotalCreditos),
            totalDebitos = todasAsDatas.Sum(c => c.TotalDebitos),
            saldoFinalPeriodo = todasAsDatas.Sum(c => c.SaldoFinal),
            totalLancamentos = todasAsDatas.Sum(c => c.QuantidadeLancamentos),
            mediaSaldoDiario = totalDias > 0 ? todasAsDatas.Average(c => c.SaldoFinal) : 0,
            maiorSaldo = todasAsDatas.Any() ? todasAsDatas.Max(c => c.SaldoFinal) : 0,
            menorSaldo = todasAsDatas.Any() ? todasAsDatas.Min(c => c.SaldoFinal) : 0,
            diasPositivos = todasAsDatas.Count(c => c.SaldoFinal > 0),
            diasNegativos = todasAsDatas.Count(c => c.SaldoFinal < 0),
            diasZerados = todasAsDatas.Count(c => c.SaldoFinal == 0)
        };

        return Ok(estatisticas);
    }
}