using MediatR;
using SmartScheduling.Domain.Entities;
using SmartScheduling.Domain.Interfaces;

namespace SmartScheduling.Application.Funcionarios.Commands.CriarFuncionario;

public sealed class CriarFuncionarioHandler(IFuncionarioRepository repo, IUnitOfWork unitOfWork)
    : IRequestHandler<CriarFuncionarioCommand, Guid>
{
    public async Task<Guid> Handle(CriarFuncionarioCommand request, CancellationToken cancellationToken)
    {
        var funcionario = Funcionario.Criar(request.Nome, request.Email, request.EstabelecimentoId);
        await repo.AddAsync(funcionario, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
        return funcionario.Id;
    }
}
