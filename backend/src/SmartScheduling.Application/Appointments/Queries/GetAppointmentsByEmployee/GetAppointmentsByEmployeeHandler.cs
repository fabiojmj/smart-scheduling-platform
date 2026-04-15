using MediatR;
using SmartScheduling.Domain.Interfaces;

namespace SmartScheduling.Application.Appointments.Queries.GetAppointmentsByEmployee;

public sealed class GetAppointmentsByEmployeeHandler(IAppointmentRepository appointmentRepo)
    : IRequestHandler<GetAppointmentsByEmployeeQuery, IReadOnlyList<AppointmentDto>>
{
    public async Task<IReadOnlyList<AppointmentDto>> Handle(GetAppointmentsByEmployeeQuery request, CancellationToken cancellationToken)
    {
        var items = await appointmentRepo.GetByEmployeeAsync(request.EmployeeId, request.Date, cancellationToken);
        return items.Select(a => new AppointmentDto(a.Id, a.Client?.Name ?? "-", a.Service?.Name ?? "-", a.TimeSlot.Start, a.TimeSlot.End, a.Status.ToString())).ToList();
    }
}
