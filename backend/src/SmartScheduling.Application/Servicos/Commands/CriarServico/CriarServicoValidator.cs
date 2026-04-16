using FluentValidation;

namespace SmartScheduling.Application.Servicos.Commands.CriarServico;

public class CriarServicoValidator : AbstractValidator<CriarServicoCommand>
{
    public CriarServicoValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(200);
        RuleFor(x => x.DuracaoMinutos).GreaterThan(0).WithMessage("Duracao deve ser maior que zero.");
        RuleFor(x => x.Preco).GreaterThanOrEqualTo(0).WithMessage("Preco nao pode ser negativo.");
        RuleFor(x => x.EstabelecimentoId).NotEmpty();
        RuleFor(x => x.Descricao).MaximumLength(1000).When(x => x.Descricao is not null);
    }
}
