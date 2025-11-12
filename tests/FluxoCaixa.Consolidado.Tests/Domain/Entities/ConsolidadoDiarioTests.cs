using FluentAssertions;
using FluxoCaixa.Consolidado.Domain.Entities;
using FluxoCaixa.Core.Domain;

namespace FluxoCaixa.Consolidado.Tests.Domain.Entities;

public class ConsolidadoDiarioTests
{
    [Fact]
    public void ConsolidadoDiario_DeveCriar_QuandoDataValida()
    {
        // Arrange
        var data = DateOnly.FromDateTime(DateTime.UtcNow);

        // Act
        var consolidado = new ConsolidadoDiario(data);

        // Assert
        consolidado.Data.Should().Be(data);
        consolidado.TotalCreditos.Should().Be(0);
        consolidado.TotalDebitos.Should().Be(0);
        consolidado.SaldoFinal.Should().Be(0);
        consolidado.QuantidadeLancamentos.Should().Be(0);
    }

    [Fact]
    public void ConsolidadoDiario_DeveLancarExcecao_QuandoDataFutura()
    {
        // Arrange
        var dataFutura = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

        // Act
        var act = () => new ConsolidadoDiario(dataFutura);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Data não pode ser futura");
    }

    [Fact]
    public void AtualizarSaldo_DeveAtualizar_QuandoDadosValidos()
    {
        // Arrange
        var consolidado = new ConsolidadoDiario(DateOnly.FromDateTime(DateTime.UtcNow));
        var totalCreditos = 1000m;
        var totalDebitos = 600m;
        var quantidadeLancamentos = 5;

        // Act
        consolidado.AtualizarSaldo(totalCreditos, totalDebitos, quantidadeLancamentos);

        // Assert
        consolidado.TotalCreditos.Should().Be(totalCreditos);
        consolidado.TotalDebitos.Should().Be(totalDebitos);
        consolidado.SaldoFinal.Should().Be(400m);
        consolidado.QuantidadeLancamentos.Should().Be(quantidadeLancamentos);
    }

    [Fact]
    public void AtualizarSaldo_DeveLancarExcecao_QuandoValoresNegativos()
    {
        // Arrange
        var consolidado = new ConsolidadoDiario(DateOnly.FromDateTime(DateTime.UtcNow));

        // Act & Assert
        var actCreditosNegativos = () => consolidado.AtualizarSaldo(-100m, 50m, 1);
        var actDebitosNegativos = () => consolidado.AtualizarSaldo(100m, -50m, 1);
        var actQuantidadeNegativa = () => consolidado.AtualizarSaldo(100m, 50m, -1);

        actCreditosNegativos.Should().Throw<DomainException>().WithMessage("Total de créditos não pode ser negativo");
        actDebitosNegativos.Should().Throw<DomainException>().WithMessage("Total de débitos não pode ser negativo");
        actQuantidadeNegativa.Should().Throw<DomainException>().WithMessage("Quantidade de lançamentos não pode ser negativa");
    }

    [Fact]
    public void AdicionarCredito_DeveIncrementar_QuandoValorValido()
    {
        // Arrange
        var consolidado = new ConsolidadoDiario(DateOnly.FromDateTime(DateTime.UtcNow));

        // Act
        consolidado.AdicionarCredito(100m);
        consolidado.AdicionarCredito(200m);

        // Assert
        consolidado.TotalCreditos.Should().Be(300m);
        consolidado.SaldoFinal.Should().Be(300m);
        consolidado.QuantidadeLancamentos.Should().Be(2);
    }

    [Fact]
    public void AdicionarDebito_DeveIncrementar_QuandoValorValido()
    {
        // Arrange
        var consolidado = new ConsolidadoDiario(DateOnly.FromDateTime(DateTime.UtcNow));

        // Act
        consolidado.AdicionarDebito(100m);
        consolidado.AdicionarDebito(50m);

        // Assert
        consolidado.TotalDebitos.Should().Be(150m);
        consolidado.SaldoFinal.Should().Be(-150m);
        consolidado.QuantidadeLancamentos.Should().Be(2);
    }

    [Fact]
    public void ConsolidadoDiario_DeveCalcularSaldoCorretamente()
    {
        // Arrange
        var consolidado = new ConsolidadoDiario(DateOnly.FromDateTime(DateTime.UtcNow));

        // Act
        consolidado.AdicionarCredito(1000m);
        consolidado.AdicionarCredito(500m);
        consolidado.AdicionarDebito(800m);
        consolidado.AdicionarDebito(200m);

        // Assert
        consolidado.TotalCreditos.Should().Be(1500m);
        consolidado.TotalDebitos.Should().Be(1000m);
        consolidado.SaldoFinal.Should().Be(500m);
        consolidado.QuantidadeLancamentos.Should().Be(4);
        consolidado.TemSaldoPositivo().Should().BeTrue();
    }

    [Fact]
    public void AdicionarCredito_DeveLancarExcecao_QuandoValorZeroOuNegativo()
    {
        // Arrange
        var consolidado = new ConsolidadoDiario(DateOnly.FromDateTime(DateTime.UtcNow));

        // Act & Assert
        var actZero = () => consolidado.AdicionarCredito(0m);
        var actNegativo = () => consolidado.AdicionarCredito(-100m);

        actZero.Should().Throw<DomainException>().WithMessage("Valor deve ser maior que zero");
        actNegativo.Should().Throw<DomainException>().WithMessage("Valor deve ser maior que zero");
    }

    [Fact]
    public void AdicionarDebito_DeveLancarExcecao_QuandoValorZeroOuNegativo()
    {
        // Arrange
        var consolidado = new ConsolidadoDiario(DateOnly.FromDateTime(DateTime.UtcNow));

        // Act & Assert
        var actZero = () => consolidado.AdicionarDebito(0m);
        var actNegativo = () => consolidado.AdicionarDebito(-100m);

        actZero.Should().Throw<DomainException>().WithMessage("Valor deve ser maior que zero");
        actNegativo.Should().Throw<DomainException>().WithMessage("Valor deve ser maior que zero");
    }

    [Fact]
    public void ConsolidadoDiario_DeveIdentificarSaldoPositivo()
    {
        // Arrange
        var consolidado = new ConsolidadoDiario(DateOnly.FromDateTime(DateTime.UtcNow));
        consolidado.AdicionarCredito(500m);
        consolidado.AdicionarDebito(200m);

        // Act & Assert
        consolidado.TemSaldoPositivo().Should().BeTrue();
        consolidado.TemSaldoNegativo().Should().BeFalse();
        consolidado.EstaZerado().Should().BeFalse();
    }

    [Fact]
    public void ConsolidadoDiario_DeveIdentificarSaldoNegativo()
    {
        // Arrange
        var consolidado = new ConsolidadoDiario(DateOnly.FromDateTime(DateTime.UtcNow));
        consolidado.AdicionarDebito(500m);
        consolidado.AdicionarCredito(200m);

        // Act & Assert
        consolidado.TemSaldoNegativo().Should().BeTrue();
        consolidado.TemSaldoPositivo().Should().BeFalse();
        consolidado.EstaZerado().Should().BeFalse();
    }

    [Fact]
    public void ConsolidadoDiario_DeveIdentificarSaldoZerado()
    {
        // Arrange
        var consolidado = new ConsolidadoDiario(DateOnly.FromDateTime(DateTime.UtcNow));
        consolidado.AdicionarCredito(500m);
        consolidado.AdicionarDebito(500m);

        // Act & Assert
        consolidado.EstaZerado().Should().BeTrue();
        consolidado.TemSaldoPositivo().Should().BeFalse();
        consolidado.TemSaldoNegativo().Should().BeFalse();
    }
}