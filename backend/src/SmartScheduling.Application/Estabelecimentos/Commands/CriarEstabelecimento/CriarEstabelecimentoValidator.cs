using FluentValidation;

namespace SmartScheduling.Application.Estabelecimentos.Commands.CriarEstabelecimento;

public class CriarEstabelecimentoValidator : AbstractValidator<CriarEstabelecimentoCommand>
{
    public CriarEstabelecimentoValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(200);
        RuleFor(x => x.WhatsAppPhoneNumberId).NotEmpty().MaximumLength(50);
        RuleFor(x => x.ProprietarioId).NotEmpty();
    }
}
