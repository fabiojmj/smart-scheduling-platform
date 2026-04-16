using SmartScheduling.Domain.Entities;

namespace SmartScheduling.Domain.Interfaces;

public interface IAgendamentoRepository : IRepository<Agendamento>
{
    Task<IReadOnlyList<Agendamento>> ObterPorFuncionarioAsync(Guid funcionarioId, DateOnly data, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Agendamento>> ObterPorClienteAsync(Guid clienteId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Agendamento>> ObterPorEstabelecimentoAsync(Guid estabelecimentoId, DateOnly data, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Agendamento>> ObterProximosAsync(TimeSpan dentro, CancellationToken cancellationToken = default);
}
