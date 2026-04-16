using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartScheduling.Application.Clientes.Commands.AtualizarCliente;
using SmartScheduling.Application.Clientes.Commands.CriarCliente;
using SmartScheduling.Application.Clientes.Queries;

namespace SmartScheduling.API.Controllers;

[ApiController]
[Route("api/clientes")]
[Authorize]
public class ClientesController(IMediator mediator) : ControllerBase
{
    /// <summary>POST /api/clientes — Cria um novo cliente</summary>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Criar([FromBody] CriarClienteCommand command, CancellationToken ct)
    {
        var id = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(ObterPorId), new { id }, id);
    }

    /// <summary>GET /api/clientes/{id} — Retorna um cliente pelo ID</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ClienteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(Guid id, CancellationToken ct)
        => Ok(await mediator.Send(new ObterClienteQuery(id), ct));

    /// <summary>GET /api/clientes?estabelecimentoId={id} — Lista clientes de um estabelecimento</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ClienteDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar([FromQuery] Guid estabelecimentoId, CancellationToken ct)
        => Ok(await mediator.Send(new ListarClientesQuery(estabelecimentoId), ct));

    /// <summary>PATCH /api/clientes/{id} — Atualiza o nome do cliente</summary>
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AtualizarNome(Guid id, [FromBody] AtualizarNomeClienteRequest request, CancellationToken ct)
    {
        await mediator.Send(new AtualizarNomeClienteCommand(id, request.Nome), ct);
        return NoContent();
    }
}

public record AtualizarNomeClienteRequest(string Nome);
