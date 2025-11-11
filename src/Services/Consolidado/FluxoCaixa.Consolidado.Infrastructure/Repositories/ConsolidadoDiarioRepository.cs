using FluxoCaixa.Core.Domain;
using FluxoCaixa.Consolidado.Domain.Entities;
using FluxoCaixa.Consolidado.Domain.Repositories;
using FluxoCaixa.Consolidado.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FluxoCaixa.Consolidado.Infrastructure.Repositories;

public class ConsolidadoDiarioRepository : IConsolidadoDiarioRepository
{
    private readonly ConsolidadoDbContext _context;

    public IUnitOfWork UnitOfWork => _context;

    public ConsolidadoDiarioRepository(ConsolidadoDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<ConsolidadoDiario?> ObterPorIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.ConsolidadosDiarios
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<ConsolidadoDiario?> ObterPorDataAsync(
        DateOnly data,
        CancellationToken cancellationToken = default)
    {
        return await _context.ConsolidadosDiarios
            .FirstOrDefaultAsync(c => c.Data == data, cancellationToken);
    }

    public async Task<IEnumerable<ConsolidadoDiario>> ObterPorPeriodoAsync(
        DateOnly dataInicio,
        DateOnly dataFim,
        CancellationToken cancellationToken = default)
    {
        return await _context.ConsolidadosDiarios
            .AsNoTracking()
            .Where(c => c.Data >= dataInicio && c.Data <= dataFim)
            .OrderBy(c => c.Data)
            .ToListAsync(cancellationToken);
    }

    public async Task AdicionarAsync(
        ConsolidadoDiario consolidado,
        CancellationToken cancellationToken = default)
    {
        await _context.ConsolidadosDiarios.AddAsync(consolidado, cancellationToken);
    }

    public void Atualizar(ConsolidadoDiario consolidado)
    {
        _context.ConsolidadosDiarios.Update(consolidado);
    }

    public async Task<bool> ExisteConsolidadoParaDataAsync(
        DateOnly data,
        CancellationToken cancellationToken = default)
    {
        return await _context.ConsolidadosDiarios
            .AnyAsync(c => c.Data == data, cancellationToken);
    }
}