using FluxoCaixa.Core.Domain;
using FluxoCaixa.Consolidado.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace FluxoCaixa.Consolidado.Infrastructure.Data;

public class ConsolidadoDbContext : DbContext, IUnitOfWork
{
    public DbSet<ConsolidadoDiario> ConsolidadosDiarios { get; set; }

    public ConsolidadoDbContext(DbContextOptions<ConsolidadoDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is Entity && (
                e.State == EntityState.Added ||
                e.State == EntityState.Modified));

        foreach (var entityEntry in entries)
        {
            if (entityEntry.State == EntityState.Modified)
            {
                ((Entity)entityEntry.Entity).AtualizadoEm = DateTime.UtcNow;
            }
        }

        var result = await base.SaveChangesAsync(cancellationToken);
        return result > 0;
    }
}