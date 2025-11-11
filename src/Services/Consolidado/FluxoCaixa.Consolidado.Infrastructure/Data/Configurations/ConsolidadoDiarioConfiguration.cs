using FluxoCaixa.Consolidado.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FluxoCaixa.Consolidado.Infrastructure.Data.Configurations;

public class ConsolidadoDiarioConfiguration : IEntityTypeConfiguration<ConsolidadoDiario>
{
    public void Configure(EntityTypeBuilder<ConsolidadoDiario> builder)
    {
        builder.ToTable("ConsolidadosDiarios");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .ValueGeneratedNever();

        builder.Property(c => c.Data)
            .IsRequired()
            .HasColumnType("date");

        builder.Property(c => c.TotalCreditos)
            .IsRequired()
            .HasColumnType("decimal(18,2)")
            .HasPrecision(18, 2);

        builder.Property(c => c.TotalDebitos)
            .IsRequired()
            .HasColumnType("decimal(18,2)")
            .HasPrecision(18, 2);

        builder.Property(c => c.SaldoFinal)
            .IsRequired()
            .HasColumnType("decimal(18,2)")
            .HasPrecision(18, 2);

        builder.Property(c => c.QuantidadeLancamentos)
            .IsRequired()
            .HasColumnType("int");

        builder.Property(c => c.CriadoEm)
            .IsRequired()
            .HasColumnType("datetime2")
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(c => c.AtualizadoEm)
            .HasColumnType("datetime2");

        builder.HasIndex(c => c.Data)
            .IsUnique()
            .HasDatabaseName("IX_ConsolidadosDiarios_Data_Unique");
    }
}