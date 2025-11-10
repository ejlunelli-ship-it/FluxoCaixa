using FluxoCaixa.Core.Domain;
using FluxoCaixa.Consolidado.Domain.Entities;

namespace FluxoCaixa.Consolidado.Domain.Repositories;

/// <summary>
/// Repositório para gerenciar consolidados diários
/// </summary>
public interface IConsolidadoDiarioRepository : IRepository<ConsolidadoDiario>
{
    Task<ConsolidadoDiario?> ObterPorIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<ConsolidadoDiario?> ObterPorDataAsync(
        DateOnly data,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<ConsolidadoDiario>> ObterPorPeriodoAsync(
        DateOnly dataInicio,
        DateOnly dataFim,
        CancellationToken cancellationToken = default);

    Task AdicionarAsync(
        ConsolidadoDiario consolidado,
        CancellationToken cancellationToken = default);

    void Atualizar(ConsolidadoDiario consolidado);

    Task<bool> ExisteConsolidadoParaDataAsync(
        DateOnly data,
        CancellationToken cancellationToken = default);
}