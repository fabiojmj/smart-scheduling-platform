namespace SmartScheduling.Domain.Events;

public sealed record AppointmentCancelledEvent(
    Guid AppointmentId,
    Guid ClientId,
    string Reason,
    DateTime CancelledAt
);
