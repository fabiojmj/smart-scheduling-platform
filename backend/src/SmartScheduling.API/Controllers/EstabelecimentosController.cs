using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartScheduling.Application.Estabelecimentos.Commands.AtualizarEstabelecimento;
using SmartScheduling.Application.Estabelecimentos.Commands.CriarEstabelecimento;
using SmartScheduling.Application.Estabelecimentos.Queries.ObterEstabelecimento;

namespace SmartScheduling.API.Controllers;

[ApiController]
[Route("api/estabelecimentos")]
[Authorize]
public class EstabelecimentosController(IMediator mediator) : ControllerBase
{
    /// <summary>POST /api/estabelecimentos — Cria um novo estabelecimento</summary>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Criar([FromBody] CriarEstabelecimentoCommand command, CancellationToken ct)
    {
        var id = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(ObterPorId), new { id }, id);
    }

    /// <summary>GET /api/estabelecimentos/meu — Retorna o estabelecimento do usuário logado</summary>
    [HttpGet("meu")]
    [ProducesResponseType(typeof(EstabelecimentoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterMeu(CancellationToken ct)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();
        var result = await mediator.Send(new ObterMeuEstabelecimentoQuery(userId), ct);
        if (result is null) return NotFound();
        return Ok(result);
    }

    /// <summary>GET /api/estabelecimentos/{id} — Retorna um estabelecimento pelo ID</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(EstabelecimentoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(Guid id, CancellationToken ct)
        => Ok(await mediator.Send(new ObterEstabelecimentoQuery(id), ct));

    /// <summary>PATCH /api/estabelecimentos/{id} — Atualiza o nome do estabelecimento</summary>
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AtualizarNome(Guid id, [FromBody] AtualizarNomeEstabelecimentoRequest request, CancellationToken ct)
    {
        await mediator.Send(new AtualizarNomeEstabelecimentoCommand(id, request.Nome), ct);
        return NoContent();
    }

    /// <summary>DELETE /api/estabelecimentos/{id} — Desativa o estabelecimento</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Desativar(Guid id, CancellationToken ct)
    {
        await mediator.Send(new DesativarEstabelecimentoCommand(id), ct);
        return NoContent();
    }
}

public record AtualizarNomeEstabelecimentoRequest(string Nome);
