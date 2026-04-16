using MediatR;
using SmartScheduling.Domain.Exceptions;
using SmartScheduling.Domain.Interfaces;

namespace SmartScheduling.Application.Servicos.Commands.AtualizarServico;

public record AtualizarPrecoCommand(Guid Id, decimal NovoPreco) : IRequest;
public record AtualizarDuracaoCommand(Guid Id, int NovosDuracaoMinutos) : IRequest;
public record DesativarServicoCommand(Guid Id) : IRequest;

public sealed class AtualizarPrecoHandler(IServicoRepository repo, IUnitOfWork unitOfWork)
    : IRequestHandler<AtualizarPrecoCommand>
{
    public async Task Handle(AtualizarPrecoCommand request, CancellationToken cancellationToken)
    {
        var servico = await repo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new DomainException("Servico nao encontrado.");
        servico.AtualizarPreco(request.NovoPreco);
        repo.Update(servico);
        await unitOfWork.CommitAsync(cancellationToken);
    }
}

public sealed class AtualizarDuracaoHandler(IServicoRepository repo, IUnitOfWork unitOfWork)
    : IRequestHandler<AtualizarDuracaoCommand>
{
    public async Task Handle(AtualizarDuracaoCommand request, CancellationToken cancellationToken)
    {
        var servico = await repo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new DomainException("Servico nao encontrado.");
        servico.AtualizarDuracao(request.NovosDuracaoMinutos);
        repo.Update(servico);
        await unitOfWork.CommitAsync(cancellationToken);
    }
}

public sealed class DesativarServicoHandler(IServicoRepository repo, IUnitOfWork unitOfWork)
    : IRequestHandler<DesativarServicoCommand>
{
    public async Task Handle(DesativarServicoCommand request, CancellationToken cancellationToken)
    {
        var servico = await repo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new DomainException("Servico nao encontrado.");
        servico.Desativar();
        repo.Update(servico);
        await unitOfWork.CommitAsync(cancellationToken);
    }
}
