using MediatR;
using SmartScheduling.Domain.Exceptions;
using SmartScheduling.Domain.Interfaces;

namespace SmartScheduling.Application.Recurring.Commands.CancelRecurringSchedule;

public sealed class CancelRecurringScheduleHandler(
    IRecurringScheduleRepository recurringRepo,
    IAgendamentoRepository appointmentRepo,
    IUnitOfWork                  unitOfWork)
    : IRequestHandler<CancelRecurringScheduleCommand>
{
    public async Task Handle(
        CancelRecurringScheduleCommand request,
        CancellationToken cancellationToken)
    {
        var schedule = await recurringRepo.GetByIdAsync(request.RecurringScheduleId, cancellationToken)
                       ?? throw new DomainException("Recorrencia nao encontrada.");

        schedule.Cancel();
        recurringRepo.Update(schedule);

        var futureAppointments = await appointmentRepo.ObterPorClienteAsync(
            schedule.ClientId, cancellationToken);

        foreach (var appt in futureAppointments.Where(a =>
            a.EstaAtivo() && a.Horario.Start > DateTime.UtcNow))
        {
            appt.Cancelar("Recorrencia cancelada pelo proprietario.");
            appointmentRepo.Update(appt);
        }

        await unitOfWork.CommitAsync(cancellationToken);
    }
}
