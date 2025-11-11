using FluentValidation;

namespace FluxoCaixa.Lancamentos.Application.Commands.AtualizarLancamento;

public class AtualizarLancamentoValidator : AbstractValidator<AtualizarLancamentoCommand>
{
    public AtualizarLancamentoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id é obrigatório");

        RuleFor(x => x.DataLancamento)
            .NotEmpty().WithMessage("Data de lançamento é obrigatória")
            .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1))
            .WithMessage("Data de lançamento não pode ser futura");

        RuleFor(x => x.Tipo)
            .IsInEnum().WithMessage("Tipo de lançamento inválido");

        RuleFor(x => x.Valor)
            .GreaterThan(0).WithMessage("Valor deve ser maior que zero");

        RuleFor(x => x.Descricao)
            .NotEmpty().WithMessage("Descrição é obrigatória")
            .MinimumLength(3).WithMessage("Descrição deve ter no mínimo 3 caracteres")
            .MaximumLength(200).WithMessage("Descrição deve ter no máximo 200 caracteres");

        RuleFor(x => x.Observacao)
            .MaximumLength(500).WithMessage("Observação deve ter no máximo 500 caracteres")
            .When(x => !string.IsNullOrWhiteSpace(x.Observacao));
    }
}