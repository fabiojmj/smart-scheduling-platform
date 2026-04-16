using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartScheduling.Application.Servicos.Commands.AtualizarServico;
using SmartScheduling.Application.Servicos.Commands.CriarServico;
using SmartScheduling.Application.Servicos.Queries;

namespace SmartScheduling.API.Controllers;

[ApiController]
[Route("api/servicos")]
[Authorize]
public class ServicosController(IMediator mediator) : ControllerBase
{
    /// <summary>POST /api/servicos — Cria um novo serviço</summary>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Criar([FromBody] CriarServicoCommand command, CancellationToken ct)
    {
        var id = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(ObterPorId), new { id }, id);
    }

    /// <summary>GET /api/servicos/{id} — Retorna um serviço pelo ID</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ServicoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(Guid id, CancellationToken ct)
        => Ok(await mediator.Send(new ObterServicoQuery(id), ct));

    /// <summary>GET /api/servicos?estabelecimentoId={id} — Lista serviços de um estabelecimento</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ServicoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar([FromQuery] Guid estabelecimentoId, CancellationToken ct)
        => Ok(await mediator.Send(new ListarServicosQuery(estabelecimentoId), ct));

    /// <summary>PATCH /api/servicos/{id}/preco — Atualiza o preço do serviço</summary>
    [HttpPatch("{id:guid}/preco")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> AtualizarPreco(Guid id, [FromBody] AtualizarPrecoRequest request, CancellationToken ct)
    {
        await mediator.Send(new AtualizarPrecoCommand(id, request.NovoPreco), ct);
        return NoContent();
    }

    /// <summary>PATCH /api/servicos/{id}/duracao — Atualiza a duração do serviço</summary>
    [HttpPatch("{id:guid}/duracao")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> AtualizarDuracao(Guid id, [FromBody] AtualizarDuracaoRequest request, CancellationToken ct)
    {
        await mediator.Send(new AtualizarDuracaoCommand(id, request.NovosDuracaoMinutos), ct);
        return NoContent();
    }

    /// <summary>DELETE /api/servicos/{id} — Desativa o serviço</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Desativar(Guid id, CancellationToken ct)
    {
        await mediator.Send(new DesativarServicoCommand(id), ct);
        return NoContent();
    }
}

public record AtualizarPrecoRequest(decimal NovoPreco);
public record AtualizarDuracaoRequest(int NovosDuracaoMinutos);
