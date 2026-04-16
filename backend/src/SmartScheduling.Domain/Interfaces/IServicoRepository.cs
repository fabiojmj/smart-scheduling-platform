using SmartScheduling.Domain.Entities;

namespace SmartScheduling.Domain.Interfaces;

public interface IServicoRepository : IRepository<Servico>
{
    Task<IReadOnlyList<Servico>> ListarPorEstabelecimentoAsync(Guid estabelecimentoId, CancellationToken cancellationToken = default);
}
