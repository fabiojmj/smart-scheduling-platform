using FluentValidation;

namespace SmartScheduling.Application.Funcionarios.Commands.CriarFuncionario;

public class CriarFuncionarioValidator : AbstractValidator<CriarFuncionarioCommand>
{
    public CriarFuncionarioValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(200);
        RuleFor(x => x.EstabelecimentoId).NotEmpty();
    }
}
