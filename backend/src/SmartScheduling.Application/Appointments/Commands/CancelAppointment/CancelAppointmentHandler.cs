using MediatR;
using SmartScheduling.Domain.Exceptions;
using SmartScheduling.Domain.Interfaces;

namespace SmartScheduling.Application.Appointments.Commands.CancelAppointment;

public sealed class CancelAppointmentHandler(IAppointmentRepository appointmentRepo, IUnitOfWork unitOfWork)
    : IRequestHandler<CancelAppointmentCommand>
{
    public async Task Handle(CancelAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await appointmentRepo.GetByIdAsync(request.AppointmentId, cancellationToken)
            ?? throw new DomainException("Agendamento nao encontrado.");
        appointment.Cancel(request.Reason);
        appointmentRepo.Update(appointment);
        await unitOfWork.CommitAsync(cancellationToken);
    }
}
