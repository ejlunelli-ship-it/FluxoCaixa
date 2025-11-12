using FluentAssertions;
using FluxoCaixa.Core.Domain;
using FluxoCaixa.Lancamentos.Domain.Entities;
using FluxoCaixa.Lancamentos.Domain.Enums;

namespace FluxoCaixa.Lancamentos.Tests.Domain.Entities;

public class LancamentoTests
{
    [Fact]
    public void Lancamento_DeveCriarCredito_QuandoDadosValidos()
    {
        // Arrange
        var dataLancamento = DateTime.UtcNow;
        var tipo = TipoLancamento.Credito;
        var valor = 100.50m;
        var descricao = "Venda de produto";

        // Act
        var lancamento = new Lancamento(dataLancamento, tipo, valor, descricao);

        // Assert
        lancamento.DataLancamento.Should().Be(dataLancamento);
        lancamento.Tipo.Should().Be(tipo);
        lancamento.Valor.Should().Be(valor);
        lancamento.Descricao.Should().Be(descricao);
        lancamento.IsCredito().Should().BeTrue();
    }

    [Fact]
    public void Lancamento_DeveCriarDebito_QuandoDadosValidos()
    {
        // Arrange
        var dataLancamento = DateTime.UtcNow;
        var tipo = TipoLancamento.Debito;
        var valor = 50.25m;
        var descricao = "Pagamento fornecedor";

        // Act
        var lancamento = new Lancamento(dataLancamento, tipo, valor, descricao);

        // Assert
        lancamento.Tipo.Should().Be(tipo);
        lancamento.IsDebito().Should().BeTrue();
    }

    [Fact]
    public void Lancamento_DeveLancarExcecao_QuandoValorZeroOuNegativo()
    {
        // Arrange
        var dataLancamento = DateTime.UtcNow;
        var tipo = TipoLancamento.Credito;
        var descricao = "Teste";

        // Act & Assert
        var actZero = () => new Lancamento(dataLancamento, tipo, 0m, descricao);
        var actNegativo = () => new Lancamento(dataLancamento, tipo, -10m, descricao);

        actZero.Should().Throw<DomainException>().WithMessage("Valor deve ser maior que zero");
        actNegativo.Should().Throw<DomainException>().WithMessage("Valor deve ser maior que zero");
    }

    [Fact]
    public void Lancamento_DeveLancarExcecao_QuandoDescricaoInvalida()
    {
        // Arrange
        var dataLancamento = DateTime.UtcNow;
        var tipo = TipoLancamento.Credito;
        var valor = 100m;

        // Act & Assert
        var actVazia = () => new Lancamento(dataLancamento, tipo, valor, "");
        var actCurta = () => new Lancamento(dataLancamento, tipo, valor, "Ab");
        var actLonga = () => new Lancamento(dataLancamento, tipo, valor, new string('A', 201));

        actVazia.Should().Throw<DomainException>().WithMessage("Descrição é obrigatória");
        actCurta.Should().Throw<DomainException>().WithMessage("Descrição deve ter no mínimo 3 caracteres");
        actLonga.Should().Throw<DomainException>().WithMessage("Descrição deve ter no máximo 200 caracteres");
    }

    [Fact]
    public void Lancamento_DeveLancarExcecao_QuandoDataFutura()
    {
        // Arrange
        var dataLancamento = DateTime.UtcNow.AddDays(2);
        var tipo = TipoLancamento.Credito;
        var valor = 100m;
        var descricao = "Teste futuro";

        // Act
        var act = () => new Lancamento(dataLancamento, tipo, valor, descricao);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Data de lançamento não pode ser futura");
    }

    [Fact]
    public void Lancamento_DeveAtualizar_QuandoDadosValidos()
    {
        // Arrange
        var lancamento = new Lancamento(
        DateTime.UtcNow, TipoLancamento.Credito, 100m, "Descrição original");

        var novaData = DateTime.UtcNow.AddDays(-1);
        var novoTipo = TipoLancamento.Debito;
        var novoValor = 200m;
        var novaDescricao = "Nova descrição";

        // Act
        lancamento.Atualizar(novaData, novoTipo, novoValor, novaDescricao);

        // Assert
        lancamento.DataLancamento.Should().Be(novaData);
        lancamento.Tipo.Should().Be(novoTipo);
        lancamento.Valor.Should().Be(novoValor);
        lancamento.Descricao.Should().Be(novaDescricao);
    }
}