using MediatR;

namespace SmartScheduling.Application.Estabelecimentos.Commands.CriarEstabelecimento;

public record CriarEstabelecimentoCommand(
    string Nome,
    string WhatsAppPhoneNumberId,
    string ProprietarioId
) : IRequest<Guid>;
