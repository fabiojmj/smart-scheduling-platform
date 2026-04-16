using MediatR;
using SmartScheduling.Domain.Entities;
using SmartScheduling.Domain.Interfaces;

namespace SmartScheduling.Application.Clientes.Commands.CriarCliente;

public sealed class CriarClienteHandler(IClienteRepository repo, IUnitOfWork unitOfWork)
    : IRequestHandler<CriarClienteCommand, Guid>
{
    public async Task<Guid> Handle(CriarClienteCommand request, CancellationToken cancellationToken)
    {
        var cliente = Cliente.Criar(request.Nome, request.Telefone, request.EstabelecimentoId, request.Email);
        await repo.AddAsync(cliente, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
        return cliente.Id;
    }
}
