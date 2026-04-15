using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartScheduling.Application.Appointments.Commands.CancelAppointment;
using SmartScheduling.Application.Appointments.Commands.ScheduleAppointment;
using SmartScheduling.Application.Appointments.Queries.GetAppointmentsByEmployee;

namespace SmartScheduling.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AppointmentsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Schedule([FromBody] ScheduleAppointmentCommand command, CancellationToken ct)
    {
        var id = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(Schedule), new { id }, id);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Cancel(Guid id, [FromBody] string reason, CancellationToken ct)
    {
        await mediator.Send(new CancelAppointmentCommand(id, reason), ct);
        return NoContent();
    }

    [HttpGet("employee/{employeeId:guid}")]
    public async Task<IActionResult> GetByEmployee(Guid employeeId, [FromQuery] DateOnly date, CancellationToken ct)
        => Ok(await mediator.Send(new GetAppointmentsByEmployeeQuery(employeeId, date), ct));
}
