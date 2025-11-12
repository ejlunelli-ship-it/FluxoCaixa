using FluentAssertions;
using FluxoCaixa.Lancamentos.Application.Commands.CriarLancamento;
using FluxoCaixa.Lancamentos.Domain.Enums;

namespace FluxoCaixa.Lancamentos.Tests.Application.Validators;

public class CriarLancamentoValidatorTests
{
    private readonly CriarLancamentoValidator _validator;

    public CriarLancamentoValidatorTests()
    {
        _validator = new CriarLancamentoValidator();
    }

    [Fact]
    public void Validator_DeveSerValido_QuandoCommandCompleto()
    {
        // Arrange
        var command = new CriarLancamentoCommand(
        DataLancamento: DateTime.UtcNow, Tipo: (int)TipoLancamento.Credito, Valor: 100.50m, Descricao: "Venda de produto", Observacao: "Cliente João");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validator_DeveFalhar_QuandoValorInvalido()
    {
        // Arrange
        var commandZero = new CriarLancamentoCommand(
        DataLancamento: DateTime.UtcNow, Tipo: (int)TipoLancamento.Credito, Valor: 0m, Descricao: "Teste");

        var commandNegativo = new CriarLancamentoCommand(
        DataLancamento: DateTime.UtcNow, Tipo: (int)TipoLancamento.Credito, Valor: -10m, Descricao: "Teste");

        // Act
        var resultZero = _validator.Validate(commandZero);
        var resultNegativo = _validator.Validate(commandNegativo);

        // Assert
        resultZero.IsValid.Should().BeFalse();
        resultNegativo.IsValid.Should().BeFalse();
        resultZero.Errors.Should().Contain(e => e.ErrorMessage == "Valor deve ser maior que zero");
    }

    [Fact]
    public void Validator_DeveFalhar_QuandoDescricaoInvalida()
    {
        // Arrange
        var commandVazia = new CriarLancamentoCommand(
        DataLancamento: DateTime.UtcNow, Tipo: (int)TipoLancamento.Credito, Valor: 100m, Descricao: "");

        var commandCurta = new CriarLancamentoCommand(
        DataLancamento: DateTime.UtcNow, Tipo: (int)TipoLancamento.Credito, Valor: 100m, Descricao: "Ab");

        // Act
        var resultVazia = _validator.Validate(commandVazia);
        var resultCurta = _validator.Validate(commandCurta);

        // Assert
        resultVazia.IsValid.Should().BeFalse();
        resultCurta.IsValid.Should().BeFalse();
        resultVazia.Errors.Should().Contain(e => e.ErrorMessage == "Descrição é obrigatória");
    }

    [Theory]
    [InlineData(1)] // Credito
    [InlineData(2)] // Debito
    public void Validator_DeveAceitarTiposValidos(int tipo)
    {
        // Arrange
        var command = new CriarLancamentoCommand(
        DataLancamento: DateTime.UtcNow, Tipo: tipo, Valor: 100m, Descricao: "Teste tipo válido");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}