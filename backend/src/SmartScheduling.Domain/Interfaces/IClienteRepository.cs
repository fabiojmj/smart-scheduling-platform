using SmartScheduling.Domain.Entities;

namespace SmartScheduling.Domain.Interfaces;

public interface IClienteRepository : IRepository<Cliente>
{
    Task<IReadOnlyList<Cliente>> ListarPorEstabelecimentoAsync(Guid estabelecimentoId, CancellationToken cancellationToken = default);
}
