using MediatR;
using SmartScheduling.Domain.Enums;
using SmartScheduling.Domain.Interfaces;

namespace SmartScheduling.Application.Recurring.Queries.GetRecurringSchedules;

public sealed class GetRecurringSchedulesHandler(
    IRecurringScheduleRepository    recurringRepo,
    IRepository<Domain.Entities.Client>   clientRepo,
    IRepository<Domain.Entities.Employee> employeeRepo,
    IRepository<Domain.Entities.Service>  serviceRepo)
    : IRequestHandler<GetRecurringSchedulesQuery, IReadOnlyList<RecurringScheduleDto>>
{
    public async Task<IReadOnlyList<RecurringScheduleDto>> Handle(
        GetRecurringSchedulesQuery request,
        CancellationToken          cancellationToken)
    {
        var schedules = await recurringRepo.GetByEstablishmentAsync(
            request.EstablishmentId, cancellationToken);

        var result = new List<RecurringScheduleDto>();

        foreach (var s in schedules)
        {
            var client   = await clientRepo.GetByIdAsync(s.ClientId, cancellationToken);
            var employee = await employeeRepo.GetByIdAsync(s.EmployeeId, cancellationToken);
            var service  = await serviceRepo.GetByIdAsync(s.ServiceId, cancellationToken);

            result.Add(new RecurringScheduleDto(
                s.Id,
                client?.Name   ?? "-",
                employee?.Name ?? "-",
                service?.Name  ?? "-",
                s.Frequency,
                s.Interval,
                s.DaysOfWeek,
                s.DayOfMonth,
                s.StartTime,
                s.StartsOn,
                s.EndsOn,
                s.MaxOccurrences,
                s.IsActive,
                BuildDescription(s)
            ));
        }

        return result;
    }

    private static string BuildDescription(Domain.Entities.RecurringSchedule s)
    {
        var days = string.Join(", ", s.DaysOfWeek.Select(d => d switch
        {
            DayOfWeek.Monday    => "Seg",
            DayOfWeek.Tuesday   => "Ter",
            DayOfWeek.Wednesday => "Qua",
            DayOfWeek.Thursday  => "Qui",
            DayOfWeek.Friday    => "Sex",
            DayOfWeek.Saturday  => "Sab",
            DayOfWeek.Sunday    => "Dom",
            _                   => d.ToString()
        }));

        return s.Frequency switch
        {
            RecurrenceFrequency.Daily   =>
                s.Interval == 1 ? $"Diario as {s.StartTime:HH:mm}"
                                : $"A cada {s.Interval} dias as {s.StartTime:HH:mm}",
            RecurrenceFrequency.Weekly  =>
                s.Interval == 1 ? $"Toda {days} as {s.StartTime:HH:mm}"
                                : $"A cada {s.Interval} semanas em {days} as {s.StartTime:HH:mm}",
            RecurrenceFrequency.Monthly =>
                s.Interval == 1 ? $"Todo dia {s.DayOfMonth} as {s.StartTime:HH:mm}"
                                : $"A cada {s.Interval} meses no dia {s.DayOfMonth} as {s.StartTime:HH:mm}",
            _ => "-"
        };
    }
}
