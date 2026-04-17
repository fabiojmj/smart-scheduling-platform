using FluentValidation;

namespace SmartScheduling.Application.Auth.Commands.RegistrarUsuario;

public class RegistrarUsuarioValidator : AbstractValidator<RegistrarUsuarioCommand>
{
    public RegistrarUsuarioValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(100).WithMessage("Nome é obrigatório (máx. 100 caracteres).");
        RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Email inválido.");
        RuleFor(x => x.Senha).NotEmpty().MinimumLength(8).WithMessage("Senha deve ter no mínimo 8 caracteres.");
    }
}
