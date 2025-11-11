using FluxoCaixa.Lancamentos.Domain.Entities;
using FluxoCaixa.Lancamentos.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FluxoCaixa.Lancamentos.Infrastructure.Data.Configurations;

public class LancamentoConfiguration : IEntityTypeConfiguration<Lancamento>
{
    public void Configure(EntityTypeBuilder<Lancamento> builder)
    {
        builder.ToTable("Lancamentos");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Id)
            .ValueGeneratedNever(); 

        builder.Property(l => l.DataLancamento)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(l => l.Tipo)
            .IsRequired()
            .HasConversion<int>()
            .HasColumnType("int");

        builder.Property(l => l.Valor)
            .IsRequired()
            .HasColumnType("decimal(18,2)")
            .HasPrecision(18, 2);

        builder.Property(l => l.Descricao)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnType("nvarchar(200)");

        builder.Property(l => l.Observacao)
            .HasMaxLength(500)
            .HasColumnType("nvarchar(500)");

        builder.Property(l => l.CriadoEm)
            .IsRequired()
            .HasColumnType("datetime2")
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(l => l.AtualizadoEm)
            .HasColumnType("datetime2");

        builder.HasIndex(l => l.DataLancamento)
            .HasDatabaseName("IX_Lancamentos_DataLancamento");

        builder.HasIndex(l => l.Tipo)
            .HasDatabaseName("IX_Lancamentos_Tipo");

        builder.HasIndex(l => new { l.DataLancamento, l.Tipo })
            .HasDatabaseName("IX_Lancamentos_DataLancamento_Tipo");
    }
}