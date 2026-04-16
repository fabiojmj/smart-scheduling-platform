using MediatR;
using SmartScheduling.Domain.Exceptions;
using SmartScheduling.Domain.Interfaces;

namespace SmartScheduling.Application.Appointments.Commands.CancelAppointment;

public sealed class CancelAppointmentHandler(IAgendamentoRepository agendamentoRepo, IUnitOfWork unitOfWork)
    : IRequestHandler<CancelAppointmentCommand>
{
    public async Task Handle(CancelAppointmentCommand request, CancellationToken cancellationToken)
    {
        var agendamento = await agendamentoRepo.GetByIdAsync(request.AppointmentId, cancellationToken)
            ?? throw new DomainException("Agendamento nao encontrado.");

        agendamento.Cancelar(request.Reason);
        agendamentoRepo.Update(agendamento);
        await unitOfWork.CommitAsync(cancellationToken);
    }
}
