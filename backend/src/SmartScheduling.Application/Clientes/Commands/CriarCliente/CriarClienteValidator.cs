using FluentValidation;

namespace SmartScheduling.Application.Clientes.Commands.CriarCliente;

public class CriarClienteValidator : AbstractValidator<CriarClienteCommand>
{
    public CriarClienteValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Telefone).NotEmpty().Matches(@"^\d{10,15}$").WithMessage("Telefone invalido. Use apenas digitos (10-15).");
        RuleFor(x => x.EstabelecimentoId).NotEmpty();
        RuleFor(x => x.Email).EmailAddress().MaximumLength(200).When(x => x.Email is not null);
    }
}
