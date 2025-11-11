using FluentValidation;

namespace FluxoCaixa.Consolidado.Application.Commands.AtualizarConsolidadoDiario;

public class AtualizarConsolidadoDiarioValidator
    : AbstractValidator<AtualizarConsolidadoDiarioCommand>
{
    public AtualizarConsolidadoDiarioValidator()
    {
        RuleFor(x => x.Data)
            .NotEmpty().WithMessage("Data é obrigatória")
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Data não pode ser futura");

        RuleFor(x => x.TotalCreditos)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Total de créditos não pode ser negativo");

        RuleFor(x => x.TotalDebitos)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Total de débitos não pode ser negativo");

        RuleFor(x => x.QuantidadeLancamentos)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Quantidade de lançamentos não pode ser negativa");
    }
}