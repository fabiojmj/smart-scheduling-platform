using MediatR;
using SmartScheduling.Domain.Exceptions;
using SmartScheduling.Domain.Interfaces;

namespace SmartScheduling.Application.Funcionarios.Commands.DesativarFuncionario;

public record DesativarFuncionarioCommand(Guid Id) : IRequest;

public sealed class DesativarFuncionarioHandler(IFuncionarioRepository repo, IUnitOfWork unitOfWork)
    : IRequestHandler<DesativarFuncionarioCommand>
{
    public async Task Handle(DesativarFuncionarioCommand request, CancellationToken cancellationToken)
    {
        var funcionario = await repo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new DomainException("Funcionario nao encontrado.");
        funcionario.Desativar();
        repo.Update(funcionario);
        await unitOfWork.CommitAsync(cancellationToken);
    }
}
