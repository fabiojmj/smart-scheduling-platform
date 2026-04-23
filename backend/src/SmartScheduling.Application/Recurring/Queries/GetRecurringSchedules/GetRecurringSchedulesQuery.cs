using MediatR;
using SmartScheduling.Domain.Enums;

namespace SmartScheduling.Application.Recurring.Queries.GetRecurringSchedules;

public record RecurringScheduleDto(
    Guid                Id,
    string              ClientName,
    string              EmployeeName,
    string              ServiceName,
    RecurrenceFrequency Frequency,
    int                 Interval,
    DayOfWeek[]         DaysOfWeek,
    int?                DayOfMonth,
    TimeOnly            StartTime,
    DateOnly            StartsOn,
    DateOnly?           EndsOn,
    int?                MaxOccurrences,
    bool                IsActive,
    string              Description
);

public record GetRecurringSchedulesQuery(Guid EstablishmentId)
    : IRequest<IReadOnlyList<RecurringScheduleDto>>;
