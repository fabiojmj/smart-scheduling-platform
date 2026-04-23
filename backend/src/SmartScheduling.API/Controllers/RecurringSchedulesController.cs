using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartScheduling.Application.Recurring.Commands.CancelRecurringSchedule;
using SmartScheduling.Application.Recurring.Commands.CreateRecurringSchedule;
using SmartScheduling.Application.Recurring.Queries.GetRecurringSchedules;

namespace SmartScheduling.API.Controllers;

[ApiController]
[Route("api/establishments/{establishmentId:guid}/recurring-schedules")]
[Authorize]
public class RecurringSchedulesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<RecurringScheduleDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        Guid establishmentId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetRecurringSchedulesQuery(establishmentId), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType<Guid>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create(
        Guid establishmentId,
        [FromBody] CreateRecurringScheduleCommand command,
        CancellationToken cancellationToken)
    {
        var id = await mediator.Send(
            command with { EstablishmentId = establishmentId }, cancellationToken);
        return CreatedAtAction(nameof(GetAll), new { establishmentId }, id);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(
        Guid id,
        CancellationToken cancellationToken)
    {
        await mediator.Send(new CancelRecurringScheduleCommand(id), cancellationToken);
        return NoContent();
    }
}
