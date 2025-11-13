using FluxoCaixa.Core.Domain;
using FluxoCaixa.Lancamentos.Domain.Entities;
using FluxoCaixa.Lancamentos.Domain.Repositories;
using FluxoCaixa.Lancamentos.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FluxoCaixa.Lancamentos.Infrastructure.Repositories;


public class LancamentoRepository : ILancamentoRepository
{
    private readonly LancamentosDbContext _context;

    public IUnitOfWork UnitOfWork => _context;

    public LancamentoRepository(LancamentosDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Lancamento?> ObterPorIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Lancamentos
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Lancamento>> ObterPorPeriodoAsync(
        DateOnly dataInicio,
        DateOnly dataFim,
        CancellationToken cancellationToken = default)
    {
        var dataInicioDateTime = dataInicio.ToDateTime(TimeOnly.MinValue); 
        var dataFimDateTime = dataFim.ToDateTime(TimeOnly.MaxValue);   

        return await _context.Lancamentos
            .AsNoTracking()
            .Where(l => l.DataLancamento >= dataInicioDateTime && l.DataLancamento <= dataFimDateTime)
            .OrderBy(l => l.DataLancamento)
            .ToListAsync(cancellationToken);
    }



    public async Task<IEnumerable<Lancamento>> ObterPorDataAsync(
        DateOnly data,
        CancellationToken cancellationToken = default)
    {
        var dataInicio = data.ToDateTime(TimeOnly.MinValue);
        var dataFim = data.ToDateTime(TimeOnly.MaxValue);

        return await _context.Lancamentos
            .AsNoTracking()
            .Where(l => l.DataLancamento >= dataInicio && l.DataLancamento <= dataFim)
            .OrderBy(l => l.DataLancamento)
            .ToListAsync(cancellationToken);
    }

    public async Task AdicionarAsync(
        Lancamento lancamento,
        CancellationToken cancellationToken = default)
    {
        await _context.Lancamentos.AddAsync(lancamento, cancellationToken);
    }

    public void Atualizar(Lancamento lancamento)
    {
        _context.Lancamentos.Update(lancamento);
    }

    public void Remover(Lancamento lancamento)
    {
        _context.Lancamentos.Remove(lancamento);
    }

    public async Task<decimal> ObterTotalCreditosPorDataAsync(
        DateOnly data,
        CancellationToken cancellationToken = default)
    {
        var dataInicio = data.ToDateTime(TimeOnly.MinValue);
        var dataFim = data.ToDateTime(TimeOnly.MaxValue);

        return await _context.Lancamentos
            .AsNoTracking()
            .Where(l => l.DataLancamento >= dataInicio &&
                       l.DataLancamento <= dataFim &&
                       l.IsCredito())
            .SumAsync(l => l.Valor, cancellationToken);
    }

    public async Task<decimal> ObterTotalDebitosPorDataAsync(
        DateOnly data,
        CancellationToken cancellationToken = default)
    {
        var dataInicio = data.ToDateTime(TimeOnly.MinValue);
        var dataFim = data.ToDateTime(TimeOnly.MaxValue);

        return await _context.Lancamentos
            .AsNoTracking()
            .Where(l => l.DataLancamento >= dataInicio &&
                       l.DataLancamento <= dataFim &&
                       l.IsDebito())
            .SumAsync(l => l.Valor, cancellationToken);
    }
}