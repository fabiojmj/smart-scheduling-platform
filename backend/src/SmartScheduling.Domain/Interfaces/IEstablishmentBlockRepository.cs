using SmartScheduling.Domain.Entities;

namespace SmartScheduling.Domain.Interfaces;

public interface IEstablishmentBlockRepository : IRepository<EstablishmentBlock>
{
    Task<IReadOnlyList<EstablishmentBlock>> ListarPorEstabelecimentoAsync(Guid estabelecimentoId, CancellationToken cancellationToken = default);
    Task<EstablishmentBlock?> ObterBloqueioAtivoAsync(Guid estabelecimentoId, DateTime data, CancellationToken cancellationToken = default);
}
