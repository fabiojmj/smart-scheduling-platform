using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartScheduling.Application.Funcionarios.Commands.AdicionarHorario;
using SmartScheduling.Application.Funcionarios.Commands.AtribuirServico;
using SmartScheduling.Application.Funcionarios.Commands.CriarFuncionario;
using SmartScheduling.Application.Funcionarios.Commands.DesativarFuncionario;
using SmartScheduling.Application.Funcionarios.Queries.ListarFuncionarios;
using SmartScheduling.Application.Funcionarios.Queries.ObterFuncionario;

namespace SmartScheduling.API.Controllers;

[ApiController]
[Route("api/funcionarios")]
[Authorize]
public class FuncionariosController(IMediator mediator) : ControllerBase
{
    /// <summary>POST /api/funcionarios — Cria um novo funcionário</summary>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Criar([FromBody] CriarFuncionarioCommand command, CancellationToken ct)
    {
        var id = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(ObterPorId), new { id }, id);
    }

    /// <summary>GET /api/funcionarios/{id} — Retorna um funcionário pelo ID</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(FuncionarioDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(Guid id, CancellationToken ct)
        => Ok(await mediator.Send(new ObterFuncionarioQuery(id), ct));

    /// <summary>GET /api/funcionarios?estabelecimentoId={id} — Lista funcionários de um estabelecimento</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<FuncionarioDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar([FromQuery] Guid estabelecimentoId, CancellationToken ct)
        => Ok(await mediator.Send(new ListarFuncionariosQuery(estabelecimentoId), ct));

    /// <summary>POST /api/funcionarios/{id}/horarios — Adiciona horário de trabalho</summary>
    [HttpPost("{id:guid}/horarios")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AdicionarHorario(Guid id, [FromBody] AdicionarHorarioRequest request, CancellationToken ct)
    {
        await mediator.Send(new AdicionarHorarioCommand(id, request.DiaSemana, request.HoraInicio, request.HoraFim), ct);
        return NoContent();
    }

    /// <summary>POST /api/funcionarios/{id}/servicos — Atribui um serviço ao funcionário</summary>
    [HttpPost("{id:guid}/servicos")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> AtribuirServico(Guid id, [FromBody] AtribuirServicoRequest request, CancellationToken ct)
    {
        await mediator.Send(new AtribuirServicoCommand(id, request.ServicoId), ct);
        return NoContent();
    }

    /// <summary>DELETE /api/funcionarios/{id} — Desativa o funcionário</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Desativar(Guid id, CancellationToken ct)
    {
        await mediator.Send(new DesativarFuncionarioCommand(id), ct);
        return NoContent();
    }
}

public record AdicionarHorarioRequest(DayOfWeek DiaSemana, TimeOnly HoraInicio, TimeOnly HoraFim);
public record AtribuirServicoRequest(Guid ServicoId);
