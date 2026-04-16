using MediatR;
using SmartScheduling.Domain.Interfaces;

namespace SmartScheduling.Application.Funcionarios.Queries.ListarFuncionarios;

public record HorarioDto(string DiaSemana, string HoraInicio, string HoraFim);

public record FuncionarioDto(
    Guid Id,
    string Nome,
    string Email,
    Guid EstabelecimentoId,
    bool Ativo,
    IReadOnlyList<HorarioDto> Horarios,
    DateTime CriadoEm
);

public record ListarFuncionariosQuery(Guid EstabelecimentoId) : IRequest<IReadOnlyList<FuncionarioDto>>;

public sealed class ListarFuncionariosHandler(IFuncionarioRepository repo)
    : IRequestHandler<ListarFuncionariosQuery, IReadOnlyList<FuncionarioDto>>
{
    public async Task<IReadOnlyList<FuncionarioDto>> Handle(ListarFuncionariosQuery request, CancellationToken cancellationToken)
    {
        var items = await repo.ListarPorEstabelecimentoAsync(request.EstabelecimentoId, cancellationToken);
        return items.Select(f => new FuncionarioDto(
            f.Id, f.Nome, f.Email, f.EstabelecimentoId, f.Ativo,
            f.Horarios.Select(h => new HorarioDto(h.DayOfWeek.ToString(), h.Start.ToString("HH:mm"), h.End.ToString("HH:mm"))).ToList(),
            f.CreatedAt
        )).ToList();
    }
}
