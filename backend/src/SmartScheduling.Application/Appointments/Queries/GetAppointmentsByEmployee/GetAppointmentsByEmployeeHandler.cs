using MediatR;
using SmartScheduling.Domain.Interfaces;

namespace SmartScheduling.Application.Appointments.Queries.GetAppointmentsByEmployee;

public sealed class GetAppointmentsByEmployeeHandler(IAgendamentoRepository agendamentoRepo)
    : IRequestHandler<GetAppointmentsByEmployeeQuery, IReadOnlyList<AppointmentDto>>
{
    public async Task<IReadOnlyList<AppointmentDto>> Handle(GetAppointmentsByEmployeeQuery request, CancellationToken cancellationToken)
    {
        var items = await agendamentoRepo.ObterPorFuncionarioAsync(request.EmployeeId, request.Date, cancellationToken);
        return items
            .Select(a => new AppointmentDto(
                a.Id,
                a.Cliente?.Nome ?? "-",
                a.Servico?.Nome ?? "-",
                a.Horario.Start,
                a.Horario.End,
                a.Status.ToString()))
            .ToList();
    }
}
