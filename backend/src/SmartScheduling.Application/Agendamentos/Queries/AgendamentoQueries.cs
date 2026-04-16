using MediatR;
using SmartScheduling.Domain.Entities;
using SmartScheduling.Domain.Exceptions;
using SmartScheduling.Domain.Interfaces;

namespace SmartScheduling.Application.Agendamentos.Queries;

public record AgendamentoDto(
    Guid Id,
    Guid ClienteId,
    string NomeCliente,
    Guid FuncionarioId,
    string NomeFuncionario,
    Guid ServicoId,
    string NomeServico,
    Guid EstabelecimentoId,
    DateTime InicioEm,
    DateTime FimEm,
    string Status,
    string? Observacoes,
    string? MotivoCancelamento,
    DateTime CriadoEm
);

public record ObterAgendamentoQuery(Guid Id) : IRequest<AgendamentoDto>;
public record ListarAgendamentosPorEstabelecimentoQuery(Guid EstabelecimentoId, DateOnly Data) : IRequest<IReadOnlyList<AgendamentoDto>>;

public sealed class ObterAgendamentoHandler(IAgendamentoRepository repo)
    : IRequestHandler<ObterAgendamentoQuery, AgendamentoDto>
{
    public async Task<AgendamentoDto> Handle(ObterAgendamentoQuery request, CancellationToken cancellationToken)
    {
        var a = await repo.ObterComDetalhesAsync(request.Id, cancellationToken)
            ?? throw new DomainException("Agendamento nao encontrado.");
        return AgendamentoMapper.Mapear(a);
    }
}

public sealed class ListarAgendamentosPorEstabelecimentoHandler(IAgendamentoRepository repo)
    : IRequestHandler<ListarAgendamentosPorEstabelecimentoQuery, IReadOnlyList<AgendamentoDto>>
{
    public async Task<IReadOnlyList<AgendamentoDto>> Handle(ListarAgendamentosPorEstabelecimentoQuery request, CancellationToken cancellationToken)
    {
        var items = await repo.ObterPorEstabelecimentoAsync(request.EstabelecimentoId, request.Data, cancellationToken);
        return items.Select(AgendamentoMapper.Mapear).ToList();
    }
}

internal static class AgendamentoMapper
{
    internal static AgendamentoDto Mapear(Agendamento a) =>
        new(
            a.Id,
            a.ClienteId,    a.Cliente?.Nome     ?? "-",
            a.FuncionarioId, a.Funcionario?.Nome ?? "-",
            a.ServicoId,    a.Servico?.Nome      ?? "-",
            a.EstabelecimentoId,
            a.Horario.Start, a.Horario.End,
            a.Status.ToString(),
            a.Observacoes,
            a.MotivoCancelamento,
            a.CreatedAt
        );
}
