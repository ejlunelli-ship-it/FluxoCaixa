using FluentValidation;

namespace FluxoCaixa.Lancamentos.Application.Commands.RemoverLancamento;

public class RemoverLancamentoValidator : AbstractValidator<RemoverLancamentoCommand>
{
    public RemoverLancamentoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id é obrigatório");
    }
}