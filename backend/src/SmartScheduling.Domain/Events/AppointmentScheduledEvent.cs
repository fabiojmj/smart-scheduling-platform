namespace SmartScheduling.Domain.Events;

public sealed record AppointmentScheduledEvent(
    Guid AppointmentId,
    Guid ClientId,
    Guid EmployeeId,
    DateTime ScheduledAt,
    string ServiceName
);
