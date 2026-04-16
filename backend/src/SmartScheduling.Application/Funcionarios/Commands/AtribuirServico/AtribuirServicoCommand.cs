using MediatR;
using SmartScheduling.Domain.Exceptions;
using SmartScheduling.Domain.Interfaces;

namespace SmartScheduling.Application.Funcionarios.Commands.AtribuirServico;

public record AtribuirServicoCommand(Guid FuncionarioId, Guid ServicoId) : IRequest;

public sealed class AtribuirServicoHandler(
    IFuncionarioRepository funcionarioRepo,
    IServicoRepository servicoRepo,
    IUnitOfWork unitOfWork)
    : IRequestHandler<AtribuirServicoCommand>
{
    public async Task Handle(AtribuirServicoCommand request, CancellationToken cancellationToken)
    {
        var funcionario = await funcionarioRepo.ObterComDetalhesAsync(request.FuncionarioId, cancellationToken)
            ?? throw new DomainException("Funcionario nao encontrado.");
        var servico = await servicoRepo.GetByIdAsync(request.ServicoId, cancellationToken)
            ?? throw new DomainException("Servico nao encontrado.");
        funcionario.AtribuirServico(servico);
        funcionarioRepo.Update(funcionario);
        await unitOfWork.CommitAsync(cancellationToken);
    }
}
