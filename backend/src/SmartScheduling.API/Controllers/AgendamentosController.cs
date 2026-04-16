using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartScheduling.Application.Agendamentos.Commands.GerenciarAgendamento;
using SmartScheduling.Application.Agendamentos.Queries;
using SmartScheduling.Application.Appointments.Commands.CancelAppointment;
using SmartScheduling.Application.Appointments.Commands.ScheduleAppointment;

namespace SmartScheduling.API.Controllers;

[ApiController]
[Route("api/agendamentos")]
[Authorize]
public class AgendamentosController(IMediator mediator) : ControllerBase
{
    /// <summary>POST /api/agendamentos — Cria um novo agendamento</summary>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Criar([FromBody] ScheduleAppointmentCommand command, CancellationToken ct)
    {
        var id = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(ObterPorId), new { id }, id);
    }

    /// <summary>GET /api/agendamentos/{id} — Retorna um agendamento pelo ID</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AgendamentoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(Guid id, CancellationToken ct)
        => Ok(await mediator.Send(new ObterAgendamentoQuery(id), ct));

    /// <summary>GET /api/agendamentos?funcionarioId={id}&amp;data={date} — Lista agendamentos por funcionário</summary>
    [HttpGet("por-funcionario")]
    [ProducesResponseType(typeof(IReadOnlyList<Application.Appointments.Queries.GetAppointmentsByEmployee.AppointmentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarPorFuncionario([FromQuery] Guid funcionarioId, [FromQuery] DateOnly data, CancellationToken ct)
        => Ok(await mediator.Send(new Application.Appointments.Queries.GetAppointmentsByEmployee.GetAppointmentsByEmployeeQuery(funcionarioId, data), ct));

    /// <summary>GET /api/agendamentos/por-estabelecimento?estabelecimentoId={id}&amp;data={date} — Lista agendamentos por estabelecimento</summary>
    [HttpGet("por-estabelecimento")]
    [ProducesResponseType(typeof(IReadOnlyList<AgendamentoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarPorEstabelecimento([FromQuery] Guid estabelecimentoId, [FromQuery] DateOnly data, CancellationToken ct)
        => Ok(await mediator.Send(new ListarAgendamentosPorEstabelecimentoQuery(estabelecimentoId, data), ct));

    /// <summary>PUT /api/agendamentos/{id}/confirmar — Confirma um agendamento pendente</summary>
    [HttpPut("{id:guid}/confirmar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Confirmar(Guid id, CancellationToken ct)
    {
        await mediator.Send(new ConfirmarAgendamentoCommand(id), ct);
        return NoContent();
    }

    /// <summary>PUT /api/agendamentos/{id}/concluir — Conclui um agendamento confirmado</summary>
    [HttpPut("{id:guid}/concluir")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Concluir(Guid id, CancellationToken ct)
    {
        await mediator.Send(new ConcluirAgendamentoCommand(id), ct);
        return NoContent();
    }

    /// <summary>PUT /api/agendamentos/{id}/cancelar — Cancela um agendamento com motivo</summary>
    [HttpPut("{id:guid}/cancelar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Cancelar(Guid id, [FromBody] CancelarRequest request, CancellationToken ct)
    {
        await mediator.Send(new CancelAppointmentCommand(id, request.Motivo), ct);
        return NoContent();
    }

    /// <summary>PUT /api/agendamentos/{id}/nao-compareceu — Marca o cliente como não compareceu</summary>
    [HttpPut("{id:guid}/nao-compareceu")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> NaoCompareceu(Guid id, CancellationToken ct)
    {
        await mediator.Send(new MarcarNaoCompareceuCommand(id), ct);
        return NoContent();
    }
}

public record CancelarRequest(string Motivo);
