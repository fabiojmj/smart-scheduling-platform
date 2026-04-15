using MediatR;

namespace SmartScheduling.Application.Appointments.Queries.GetAppointmentsByEmployee;

public record AppointmentDto(Guid Id, string ClientName, string ServiceName, DateTime Start, DateTime End, string Status);
public record GetAppointmentsByEmployeeQuery(Guid EmployeeId, DateOnly Date) : IRequest<IReadOnlyList<AppointmentDto>>;
