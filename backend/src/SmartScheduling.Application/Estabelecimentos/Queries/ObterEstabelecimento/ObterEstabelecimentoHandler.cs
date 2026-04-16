using MediatR;
using SmartScheduling.Domain.Exceptions;
using SmartScheduling.Domain.Interfaces;

namespace SmartScheduling.Application.Estabelecimentos.Queries.ObterEstabelecimento;

public sealed class ObterEstabelecimentoHandler(IEstabelecimentoRepository repo)
    : IRequestHandler<ObterEstabelecimentoQuery, EstabelecimentoDto>
{
    public async Task<EstabelecimentoDto> Handle(ObterEstabelecimentoQuery request, CancellationToken cancellationToken)
    {
        var e = await repo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new DomainException("Estabelecimento nao encontrado.");
        return new EstabelecimentoDto(e.Id, e.Nome, e.WhatsAppPhoneNumberId, e.Ativo, e.CreatedAt);
    }
}
