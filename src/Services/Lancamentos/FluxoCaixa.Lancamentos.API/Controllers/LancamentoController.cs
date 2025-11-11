using FluxoCaixa.Lancamentos.Application.Commands.AtualizarLancamento;
using FluxoCaixa.Lancamentos.Application.Commands.CriarLancamento;
using FluxoCaixa.Lancamentos.Application.Commands.RemoverLancamento;
using FluxoCaixa.Lancamentos.Application.Queries.ObterLancamentoPorId;
using FluxoCaixa.Lancamentos.Application.Queries.ObterLancamentosPorPeriodo;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FluxoCaixa.Lancamentos.API.Controllers;

/// <summary>
/// Controller para gerenciar lançamentos financeiros
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class LancamentosController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<LancamentosController> _logger;

    public LancamentosController(IMediator mediator, ILogger<LancamentosController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Cria um novo lançamento (crédito ou débito)
    /// </summary>
    /// <param name="command">Dados do lançamento</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lançamento criado</returns>
    /// <response code="201">Lançamento criado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost]
    [ProducesResponseType(typeof(CriarLancamentoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin,Operador")]
    public async Task<ActionResult<CriarLancamentoResponse>> CriarLancamento(
        [FromBody] CriarLancamentoCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Criando novo lançamento: {Tipo} - Valor: {Valor}", command.Tipo, command.Valor);

        try
        {
            var result = await _mediator.Send(command, cancellationToken);

            _logger.LogInformation("Lançamento criado com sucesso: {Id}", result.Id);

            return CreatedAtAction(
                nameof(ObterPorId),
                new { id = result.Id },
                result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar lançamento");
            return StatusCode(500, new { error = "Erro ao criar lançamento", message = ex.Message });
        }
    }

    /// <summary>
    /// Obtém um lançamento por ID
    /// </summary>
    /// <param name="id">ID do lançamento</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lançamento encontrado</returns>
    /// <response code="200">Lançamento encontrado</response>
    /// <response code="404">Lançamento não encontrado</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin,Operador")]
    public async Task<ActionResult> ObterPorId(
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Buscando lançamento: {Id}", id);

        var query = new ObterLancamentoPorIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (result == null)
        {
            _logger.LogWarning("Lançamento não encontrado: {Id}", id);
            return NotFound(new { message = $"Lançamento {id} não encontrado" });
        }

        return Ok(result);
    }

    /// <summary>
    /// Obtém lançamentos por período
    /// </summary>
    /// <param name="dataInicio">Data inicial</param>
    /// <param name="dataFim">Data final</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de lançamentos</returns>
    /// <response code="200">Lista de lançamentos</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize(Roles = "Admin,Operador")]
    public async Task<ActionResult> ObterPorPeriodo(
        [FromQuery] DateTime dataInicio,
        [FromQuery] DateTime dataFim,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Buscando lançamentos entre {DataInicio} e {DataFim}", dataInicio, dataFim);

        var query = new ObterLancamentosPorPeriodoQuery(dataInicio, dataFim);
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Atualiza um lançamento existente
    /// </summary>
    /// <param name="id">ID do lançamento</param>
    /// <param name="command">Dados atualizados</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lançamento atualizado</returns>
    /// <response code="200">Lançamento atualizado com sucesso</response>
    /// <response code="404">Lançamento não encontrado</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin,Operador")]
    public async Task<ActionResult> AtualizarLancamento(
        Guid id,
        [FromBody] AtualizarLancamentoCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            return BadRequest(new { message = "ID da URL não corresponde ao ID do corpo da requisição" });
        }

        _logger.LogInformation("Atualizando lançamento: {Id}", id);

        try
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar lançamento: {Id}", id);
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Remove um lançamento
    /// </summary>
    /// <param name="id">ID do lançamento</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Confirmação de remoção</returns>
    /// <response code="200">Lançamento removido com sucesso</response>
    /// <response code="404">Lançamento não encontrado</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> RemoverLancamento(
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Removendo lançamento: {Id}", id);

        var command = new RemoverLancamentoCommand(id);

        try
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover lançamento: {Id}", id);
            return NotFound(new { message = ex.Message });
        }
    }
}