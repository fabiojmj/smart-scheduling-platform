using MediatR;
using SmartScheduling.Domain.Entities;
using SmartScheduling.Domain.Enums;
using SmartScheduling.Domain.Exceptions;
using SmartScheduling.Domain.Interfaces;
using SmartScheduling.Domain.ValueObjects;

namespace SmartScheduling.Application.Recurring.Commands.CreateRecurringSchedule;

public sealed class CreateRecurringScheduleHandler(
    IRecurringScheduleRepository    recurringRepo,
    IAgendamentoRepository          appointmentRepo,
    IRepository<Cliente>            clientRepo,
    IRepository<Funcionario>        employeeRepo,
    IRepository<Servico>            serviceRepo,
    IEstablishmentAvailabilityService availabilityService,
    IUnitOfWork                     unitOfWork)
    : IRequestHandler<CreateRecurringScheduleCommand, Guid>
{
    private const int MaxUpfrontOccurrences = 12;

    public async Task<Guid> Handle(
        CreateRecurringScheduleCommand request,
        CancellationToken cancellationToken)
    {
        var cliente    = await clientRepo.GetByIdAsync(request.ClientId, cancellationToken)
                         ?? throw new DomainException("Cliente nao encontrado.");
        var funcionario = await employeeRepo.GetByIdAsync(request.EmployeeId, cancellationToken)
                         ?? throw new DomainException("Funcionario nao encontrado.");
        var servico    = await serviceRepo.GetByIdAsync(request.ServiceId, cancellationToken)
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

        foreach (var date in occurrences)
        {
            var startDateTime = date.ToDateTime(request.StartTime);

            var disponivel = await availabilityService.EstaDisponivelAsync(
                request.EstablishmentId, startDateTime, cancellationToken);

            if (!disponivel)
                continue;

            var slot        = TimeSlot.Create(startDateTime, servico.DuracaoMinutos);
            var agendamento = Agendamento.Agendar(cliente, funcionario, servico, slot);
            agendamento.Confirmar();

            await appointmentRepo.AddAsync(agendamento, cancellationToken);
        }

        await unitOfWork.CommitAsync(cancellationToken);
        return schedule.Id;
    }
}
