using MediatR;

namespace SmartScheduling.Application.Appointments.Commands.CancelAppointment;

public record CancelAppointmentCommand(Guid AppointmentId, string Reason) : IRequest;
