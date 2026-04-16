using MediatR;
using SmartScheduling.Domain.Exceptions;
using SmartScheduling.Domain.Interfaces;

namespace SmartScheduling.Application.Agendamentos.Commands.GerenciarAgendamento;

public record ConfirmarAgendamentoCommand(Guid Id) : IRequest;
public record ConcluirAgendamentoCommand(Guid Id) : IRequest;
public record MarcarNaoCompareceuCommand(Guid Id) : IRequest;

public sealed class ConfirmarAgendamentoHandler(IAgendamentoRepository repo, IUnitOfWork unitOfWork)
    : IRequestHandler<ConfirmarAgendamentoCommand>
{
    public async Task Handle(ConfirmarAgendamentoCommand request, CancellationToken cancellationToken)
    {
        var agendamento = await repo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new DomainException("Agendamento nao encontrado.");
        agendamento.Confirmar();
        repo.Update(agendamento);
        await unitOfWork.CommitAsync(cancellationToken);
    }
}

public sealed class ConcluirAgendamentoHandler(IAgendamentoRepository repo, IUnitOfWork unitOfWork)
    : IRequestHandler<ConcluirAgendamentoCommand>
{
    public async Task Handle(ConcluirAgendamentoCommand request, CancellationToken cancellationToken)
    {
        var agendamento = await repo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new DomainException("Agendamento nao encontrado.");
        agendamento.Concluir();
        repo.Update(agendamento);
        await unitOfWork.CommitAsync(cancellationToken);
    }
}

public sealed class MarcarNaoCompareceuHandler(IAgendamentoRepository repo, IUnitOfWork unitOfWork)
    : IRequestHandler<MarcarNaoCompareceuCommand>
{
    public async Task Handle(MarcarNaoCompareceuCommand request, CancellationToken cancellationToken)
    {
        var agendamento = await repo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new DomainException("Agendamento nao encontrado.");
        agendamento.MarcarNaoCompareceu();
        repo.Update(agendamento);
        await unitOfWork.CommitAsync(cancellationToken);
    }
}
