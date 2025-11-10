using FluxoCaixa.Core.Domain;
using FluxoCaixa.Lancamentos.Domain.Entities;

namespace FluxoCaixa.Lancamentos.Domain.Repositories;

/// <summary>
/// Repositório para gerenciar lançamentos
/// </summary>
public interface ILancamentoRepository : IRepository<Lancamento>
{
    Task<Lancamento?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IEnumerable<Lancamento>> ObterPorPeriodoAsync(
        DateTime dataInicio,
        DateTime dataFim,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Lancamento>> ObterPorDataAsync(
        DateOnly data,
        CancellationToken cancellationToken = default);

    Task AdicionarAsync(Lancamento lancamento, CancellationToken cancellationToken = default);

    void Atualizar(Lancamento lancamento);

    void Remover(Lancamento lancamento);

    Task<decimal> ObterTotalCreditosPorDataAsync(
        DateOnly data,
        CancellationToken cancellationToken = default);

    Task<decimal> ObterTotalDebitosPorDataAsync(
        DateOnly data,
        CancellationToken cancellationToken = default);
}