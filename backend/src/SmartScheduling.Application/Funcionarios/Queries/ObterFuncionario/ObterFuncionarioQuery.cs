using MediatR;
using SmartScheduling.Application.Funcionarios.Queries.ListarFuncionarios;
using SmartScheduling.Domain.Exceptions;
using SmartScheduling.Domain.Interfaces;

namespace SmartScheduling.Application.Funcionarios.Queries.ObterFuncionario;

public record ObterFuncionarioQuery(Guid Id) : IRequest<FuncionarioDto>;

public sealed class ObterFuncionarioHandler(IFuncionarioRepository repo)
    : IRequestHandler<ObterFuncionarioQuery, FuncionarioDto>
{
    public async Task<FuncionarioDto> Handle(ObterFuncionarioQuery request, CancellationToken cancellationToken)
    {
        var f = await repo.ObterComDetalhesAsync(request.Id, cancellationToken)
            ?? throw new DomainException("Funcionario nao encontrado.");
        return new FuncionarioDto(
            f.Id, f.Nome, f.Email, f.EstabelecimentoId, f.Ativo,
            f.Horarios.Select(h => new HorarioDto(h.DayOfWeek.ToString(), h.Start.ToString("HH:mm"), h.End.ToString("HH:mm"))).ToList(),
            f.CreatedAt
        );
    }
}
