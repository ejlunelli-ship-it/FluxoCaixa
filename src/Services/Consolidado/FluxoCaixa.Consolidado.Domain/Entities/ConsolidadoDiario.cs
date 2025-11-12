using FluxoCaixa.Core.Domain;

namespace FluxoCaixa.Consolidado.Domain.Entities;

/// <summary>
/// Representa o consolidado financeiro de um dia específico
/// </summary>
public class ConsolidadoDiario : Entity, IAggregateRoot
{
    public DateOnly Data { get; private set; }
    public decimal TotalCreditos { get; private set; }
    public decimal TotalDebitos { get; private set; }
    public decimal SaldoFinal { get; private set; }
    public int QuantidadeLancamentos { get; private set; }

    private ConsolidadoDiario() { }

    public ConsolidadoDiario(DateOnly data)
    {
        if (data > DateOnly.FromDateTime(DateTime.UtcNow))
            throw new DomainException("Data não pode ser futura");

        Data = data;
        TotalCreditos = 0;
        TotalDebitos = 0;
        SaldoFinal = 0;
        QuantidadeLancamentos = 0;
    }

    public void AtualizarSaldo(
        decimal totalCreditos,
        decimal totalDebitos,
        int quantidadeLancamentos)
    {
        if (totalCreditos < 0)
            throw new DomainException("Total de créditos não pode ser negativo");

        if (totalDebitos < 0)
            throw new DomainException("Total de débitos não pode ser negativo");

        if (quantidadeLancamentos < 0)
            throw new DomainException("Quantidade de lançamentos não pode ser negativa");

        TotalCreditos = totalCreditos;
        TotalDebitos = totalDebitos;
        SaldoFinal = totalCreditos - totalDebitos;
        QuantidadeLancamentos = quantidadeLancamentos;
        AtualizadoEm = DateTime.UtcNow;
    }

    public void AdicionarCredito(decimal valor)
    {
        if (valor <= 0)
            throw new DomainException("Valor deve ser maior que zero");

        TotalCreditos += valor;
        SaldoFinal = TotalCreditos - TotalDebitos;
        QuantidadeLancamentos++;
        AtualizadoEm = DateTime.UtcNow;
    }

    public void AdicionarDebito(decimal valor)
    {
        if (valor <= 0)
            throw new DomainException("Valor deve ser maior que zero");

        TotalDebitos += valor;
        SaldoFinal = TotalCreditos - TotalDebitos;
        QuantidadeLancamentos++;
        AtualizadoEm = DateTime.UtcNow;
    }

    public bool TemSaldoPositivo() => SaldoFinal > 0;
    public bool TemSaldoNegativo() => SaldoFinal < 0;
    public bool EstaZerado() => SaldoFinal == 0;
}