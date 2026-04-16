using MediatR;

namespace SmartScheduling.Application.Estabelecimentos.Queries.ObterEstabelecimento;

public record EstabelecimentoDto(
    Guid Id,
    string Nome,
    string WhatsAppPhoneNumberId,
    bool Ativo,
    DateTime CriadoEm
);

public record ObterEstabelecimentoQuery(Guid Id) : IRequest<EstabelecimentoDto>;
