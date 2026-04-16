using MediatR;
using SmartScheduling.Domain.Entities;
using SmartScheduling.Domain.Interfaces;

namespace SmartScheduling.Application.Estabelecimentos.Commands.CriarEstabelecimento;

public sealed class CriarEstabelecimentoHandler(IEstabelecimentoRepository repo, IUnitOfWork unitOfWork)
    : IRequestHandler<CriarEstabelecimentoCommand, Guid>
{
    public async Task<Guid> Handle(CriarEstabelecimentoCommand request, CancellationToken cancellationToken)
    {
        var estabelecimento = Estabelecimento.Criar(request.Nome, request.WhatsAppPhoneNumberId, request.ProprietarioId);
        await repo.AddAsync(estabelecimento, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
        return estabelecimento.Id;
    }
}
