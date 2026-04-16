using MediatR;
using SmartScheduling.Domain.Entities;
using SmartScheduling.Domain.Interfaces;

namespace SmartScheduling.Application.Servicos.Commands.CriarServico;

public sealed class CriarServicoHandler(IServicoRepository repo, IUnitOfWork unitOfWork)
    : IRequestHandler<CriarServicoCommand, Guid>
{
    public async Task<Guid> Handle(CriarServicoCommand request, CancellationToken cancellationToken)
    {
        var servico = Servico.Criar(request.Nome, request.DuracaoMinutos, request.Preco, request.EstabelecimentoId, request.Descricao);
        await repo.AddAsync(servico, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
        return servico.Id;
    }
}
