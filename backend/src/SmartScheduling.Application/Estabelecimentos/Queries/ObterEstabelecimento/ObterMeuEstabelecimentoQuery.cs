using MediatR;
using SmartScheduling.Domain.Interfaces;

namespace SmartScheduling.Application.Estabelecimentos.Queries.ObterEstabelecimento;

public record ObterMeuEstabelecimentoQuery(string ProprietarioId) : IRequest<EstabelecimentoDto?>;

public sealed class ObterMeuEstabelecimentoHandler(IEstabelecimentoRepository repo)
    : IRequestHandler<ObterMeuEstabelecimentoQuery, EstabelecimentoDto?>
{
    public async Task<EstabelecimentoDto?> Handle(ObterMeuEstabelecimentoQuery request, CancellationToken cancellationToken)
    {
        var e = await repo.ObterPorProprietarioAsync(request.ProprietarioId, cancellationToken);
        if (e is null) return null;
        return new EstabelecimentoDto(e.Id, e.Nome, e.WhatsAppPhoneNumberId, e.Ativo, e.CreatedAt);
    }
}
