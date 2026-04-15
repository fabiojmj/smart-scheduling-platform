using MediatR;

namespace SmartScheduling.Application.Appointments.Commands.ScheduleAppointment;

public record ScheduleAppointmentCommand(Guid ClientId, Guid EmployeeId, Guid ServiceId, DateTime Start, string? Notes) : IRequest<Guid>;
