using FluxoCaixa.Core.Domain;
using FluxoCaixa.Lancamentos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace FluxoCaixa.Lancamentos.Infrastructure.Data;

public class LancamentosDbContext : DbContext, IUnitOfWork
{
    public DbSet<Lancamento> Lancamentos { get; set; }

    public LancamentosDbContext(DbContextOptions<LancamentosDbContext> options)
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