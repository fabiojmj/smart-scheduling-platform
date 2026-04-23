using MediatR;
using SmartScheduling.Domain.Entities;
using SmartScheduling.Domain.Enums;
using SmartScheduling.Domain.Exceptions;
using SmartScheduling.Domain.Interfaces;
using SmartScheduling.Domain.Services;
using SmartScheduling.Domain.ValueObjects;

namespace SmartScheduling.Application.Recurring.Commands.CreateRecurringSchedule;

public sealed class CreateRecurringScheduleHandler(
    IRecurringScheduleRepository    recurringRepo,
    IAppointmentRepository          appointmentRepo,
    IRepository<Client>             clientRepo,
    IRepository<Employee>           employeeRepo,
    IRepository<Service>            serviceRepo,
    IEstablishmentWorkingHoursRepository workingHoursRepo,
    IEstablishmentBlockRepository   blockRepo,
    EstablishmentAvailabilityService availabilityService,
    IUnitOfWork                     unitOfWork)
    : IRequestHandler<CreateRecurringScheduleCommand, Guid>
{
    private const int MaxUpfrontOccurrences = 12;

    public async Task<Guid> Handle(
        CreateRecurringScheduleCommand request,
        CancellationToken cancellationToken)
    {
        var client   = await clientRepo.GetByIdAsync(request.ClientId, cancellationToken)
                       ?? throw new DomainException("Cliente nao encontrado.");
        var employee = await employeeRepo.GetByIdAsync(request.EmployeeId, cancellationToken)
                       ?? throw new DomainException("Funcionario nao encontrado.");
        var service  = await serviceRepo.GetByIdAsync(request.ServiceId, cancellationToken)
                       ?? throw new DomainException("Servico nao encontrado.");

        var schedule = request.Frequency switch
        {
            RecurrenceFrequency.Weekly  => RecurringSchedule.CreateWeekly(
                request.EstablishmentId, request.ClientId, request.EmployeeId,
                request.ServiceId, request.DaysOfWeek, request.StartTime,
                request.StartsOn, request.EndsOn, request.MaxOccurrences, request.Interval),

            RecurrenceFrequency.Daily   => RecurringSchedule.CreateDaily(
                request.EstablishmentId, request.ClientId, request.EmployeeId,
                request.ServiceId, request.StartTime, request.StartsOn,
                request.EndsOn, request.MaxOccurrences, request.Interval),

            RecurrenceFrequency.Monthly => RecurringSchedule.CreateMonthly(
                request.EstablishmentId, request.ClientId, request.EmployeeId,
                request.ServiceId, request.DayOfMonth!.Value, request.StartTime,
                request.StartsOn, request.EndsOn, request.MaxOccurrences, request.Interval),

            _ => throw new DomainException("Frequencia de recorrencia invalida.")
        };

        await recurringRepo.AddAsync(schedule, cancellationToken);

        var until       = request.StartsOn.AddMonths(3);
        var occurrences = schedule.GenerateOccurrences(request.StartsOn, until, MaxUpfrontOccurrences);

        var workingHours = await workingHoursRepo.GetByEstablishmentAsync(
            request.EstablishmentId, cancellationToken);

        foreach (var date in occurrences)
        {
            var startDateTime = date.ToDateTime(request.StartTime);
            var blocks        = await blockRepo.GetActiveBlocksAsync(
                request.EstablishmentId, date, cancellationToken);

            try
            {
                availabilityService.EnsureOpen(workingHours, blocks, startDateTime);
            }
            catch (DomainException)
            {
                continue;
            }

            var slot        = TimeSlot.Create(startDateTime, service.DurationMinutes);
            var appointment = Appointment.Schedule(client, employee, service, slot);
            appointment.Confirm();

            await appointmentRepo.AddAsync(appointment, cancellationToken);
        }

        await unitOfWork.CommitAsync(cancellationToken);
        return schedule.Id;
    }
}
