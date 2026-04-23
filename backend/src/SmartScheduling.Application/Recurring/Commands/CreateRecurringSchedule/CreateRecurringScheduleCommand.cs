using MediatR;
using SmartScheduling.Domain.Enums;

namespace SmartScheduling.Application.Recurring.Commands.CreateRecurringSchedule;

public record CreateRecurringScheduleCommand(
    Guid                 EstablishmentId,
    Guid                 ClientId,
    Guid                 EmployeeId,
    Guid                 ServiceId,
    RecurrenceFrequency  Frequency,
    int                  Interval,
    DayOfWeek[]          DaysOfWeek,
    int?                 DayOfMonth,
    TimeOnly             StartTime,
    DateOnly             StartsOn,
    DateOnly?            EndsOn,
    int?                 MaxOccurrences
) : IRequest<Guid>;
