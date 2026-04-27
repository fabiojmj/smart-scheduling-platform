using MediatR;
using SmartScheduling.Domain.Exceptions;
using SmartScheduling.Domain.Interfaces;

namespace SmartScheduling.Application.Estabelecimentos.Commands.PrimeiroAtendimento;

public record DefinirFuncionarioPrimeiroAtendimentoCommand(
    Guid EstabelecimentoId,
    Guid? FuncionarioId
) : IRequest;

public sealed class DefinirFuncionarioPrimeiroAtendimentoHandler(
    IEstabelecimentoRepository estabelecimentoRepo,
    IFuncionarioRepository funcionarioRepo,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DefinirFuncionarioPrimeiroAtendimentoCommand>
{
    public async Task Handle(DefinirFuncionarioPrimeiroAtendimentoCommand request, CancellationToken cancellationToken)
    {
        var estabelecimento = await estabelecimentoRepo.GetByIdAsync(request.EstabelecimentoId, cancellationToken)
            ?? throw new DomainException("Estabelecimento nao encontrado.");

        if (request.FuncionarioId.HasValue)
        {
            var funcionario = await funcionarioRepo.GetByIdAsync(request.FuncionarioId.Value, cancellationToken)
                ?? throw new DomainException("Funcionario nao encontrado.");

            if (funcionario.EstabelecimentoId != request.EstabelecimentoId)
                throw new DomainException("Funcionario nao pertence a este estabelecimento.");

            if (!funcionario.Ativo)
                throw new DomainException("Funcionario inativo nao pode ser responsavel pelo primeiro atendimento.");
        }

        estabelecimento.DefinirFuncionarioPrimeiroAtendimento(request.FuncionarioId);
        estabelecimentoRepo.Update(estabelecimento);
        await unitOfWork.CommitAsync(cancellationToken);
    }
}
